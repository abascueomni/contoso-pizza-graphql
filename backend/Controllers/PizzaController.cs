using ContosoPizza.Data;
using ContosoPizza.DTOs;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/pizza")]
public class PizzasController : ControllerBase
{
    private readonly PizzaContext _db;
    public PizzasController(PizzaContext db) => _db = db;

    // GET /api/pizzas
    [HttpGet]
    public async Task<ActionResult<List<PizzaDto>>> GetAll([FromQuery] bool includeDeleted = false)
    {
        var pizzas = _db.Pizzas
            .Include(p => p.Toppings)
            .Where(p => !p.IsDeleted)
            .ToList();

        if (!includeDeleted)
            pizzas = pizzas.Where(p => !p.IsDeleted).ToList();

        return Ok(pizzas.Select(MapToDto));
    }

    // GET /api/pizzas/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PizzaDto>> GetById(int id)
    {
        var pizza = await _db.Pizzas
            .Include(p => p.Toppings)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (pizza == null || pizza.IsDeleted) return NotFound();
        return Ok(MapToDto(pizza));
    }

    // GET /api/pizzas/byname/{name}
    [HttpGet("byname/{name}")]
    public async Task<ActionResult<PizzaDto>> GetByName(string name)
    {
        var pizza = await _db.Pizzas
            .Include(p => p.Toppings)
            .FirstOrDefaultAsync(p => p.Name == name && !p.IsDeleted);
        if (pizza == null) return NotFound();
        return Ok(MapToDto(pizza));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var pizza = _db.Pizzas.Find(id);
        if (pizza == null) return NotFound();

        pizza.IsDeleted = true;
        _db.SaveChanges();

        return NoContent();
    }

    // POST /api/pizzas/custom
    [HttpPost("custom")]
    public IActionResult CreateCustomPizza([FromBody] CreateCustomPizzaRequest request)
    {
        // Handle menu preset first
        if (request.MenuPreset != MenuPreset.None)
        {
            // Lookup preset pizza in DB
            var presetPizza = _db.Pizzas
                .FirstOrDefault(p => p.IsMenuPizza && p.Name == request.MenuPreset.ToString());

            if (presetPizza == null)
                return NotFound($"Menu preset '{request.MenuPreset}' not found.");

            return Ok(MapToDto(presetPizza));
        }

        // Validate toppings
        var invalidToppings = request.Toppings?
            .Where(t => !Enum.IsDefined(typeof(Topping), t))
            .ToList();


        if (invalidToppings != null && invalidToppings.Any())
            return BadRequest($"Invalid topping(s): {string.Join(", ", invalidToppings)}");

        if (request.Toppings == null || !request.Toppings.Any())
        {
            return BadRequest("Toppings are required.");
        }
        var alreadyExistingPizza = _db.Pizzas
            .Include(p => p.Toppings) // make sure toppings are loaded
            .Where(p => p.IsMenuPizza)
            .AsEnumerable() // still ok for small sets
            .Any(p =>
                p.Toppings.Count == request.Toppings.Count() &&
                request.Toppings.All(t => p.Toppings.Any(pt => pt.Topping == t))
            );


        if (alreadyExistingPizza)
        {
            return BadRequest("A menu pizza with the same toppings already exists.");
        }

        // Create custom pizza
        var customPizza = new Pizza
        {
            Name = request.Name ?? "Custom Pizza",
            IsGlutenFree = request.IsGlutenFree,
            IsMenuPizza = false,
            Toppings = request.Toppings?.Select(t => new PizzaTopping
            {
                Topping = t
            }).ToList() ?? new List<PizzaTopping>(),
            Price = CalculatePrice(request) // see below
        };

        _db.Pizzas.Add(customPizza);
        _db.SaveChanges();

        return Ok(MapToDto(customPizza));
    }
    private double CalculatePrice(CreateCustomPizzaRequest request)
    {
        double price = 5.0; // base price for a plain pizza

        if (request.IsGlutenFree)
            price += 1.15;

        price += (request.Toppings?.Count() ?? 0) * 0.75;

        return price;
    }


    private PizzaDto MapToDto(Pizza pizza)
    {
        return new PizzaDto
        {
            Id = pizza.Id,
            Name = pizza.Name ?? "Custom Pizza",
            Price = pizza.Price,
            IsDeleted = pizza.IsDeleted,
            IsMenuPizza = pizza.IsMenuPizza,
            IsGlutenFree = pizza.IsGlutenFree,
            Toppings = pizza.Toppings.Select(pt => pt.Topping).ToList() // use the list of enums
        };
    }

}
public static class ToppingValidator
{
    public static readonly HashSet<string> AllowedToppings = Enum
        .GetNames(typeof(Topping))
        .ToHashSet();

    public static bool IsValid(Topping topping) => AllowedToppings.Contains(topping.ToString());
}
