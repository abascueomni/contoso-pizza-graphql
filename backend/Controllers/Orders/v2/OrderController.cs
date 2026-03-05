using ContosoPizza.Data;
using ContosoPizza.DTOs;
using ContosoPizza.DTOs.Orders.V2;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Controllers.Orders.V2;

[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : ControllerBase
{
    private readonly PizzaContext _db;
    public OrdersController(PizzaContext db) => _db = db;

    // GET /api/orders
    [HttpGet]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
    {
        var orders = await _db.Orders
            .Include(o => o.Pizzas)
            .ThenInclude(op => op.Pizza)
            .ToListAsync();

        return Ok(orders.Select(order => MapToDto(order)).ToList());
    }

    // GET /api/orders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var order = await _db.Orders.Include(o => o.Pizzas)
                                    .ThenInclude(op => op.Pizza)
                                    .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return Ok(MapToDto(order));
    }

    // POST /api/orders
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create([FromBody] CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            Coupon = request.Coupon
        };

        foreach (var pq in request.Pizzas)
        {
            var pizza = await _db.Pizzas.FindAsync(pq.PizzaId);
            if (pizza == null) return BadRequest($"Pizza with Id {pq.PizzaId} not found");

            order.Pizzas.Add(new OrderPizza
            {
                Pizza = pizza,
                Quantity = pq.Quantity
            });
        }
        foreach (var dq in request.Drinks)
        {
            order.Drinks.Add(new DrinkQuantity() { DrinkName = dq.DrinkName, Quantity = dq.Quantity });
        }

        _db.Orders.Add(order);
        int pizzasInQueue = _db.OrderPizzas.Sum(op => op.Quantity)
                             + order.Pizzas.Sum(op => op.Quantity); // include current order
        int timeToWait = 10 + (pizzasInQueue * 10 / 2);
        order.PickUpTime = DateTime.UtcNow.AddMinutes(timeToWait);

        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToDto(order));
    }
    private double CalculateTotal(Order order)
    {
        // No coupon? return subtotal
        if (string.IsNullOrEmpty(order.Coupon))
            return order.SubTotal;

        // Lookup coupon in DB
        var foundCoupon = _db.Coupons.FirstOrDefault(c => c.CouponCode == order.Coupon);

        if (foundCoupon != null)
        {
            return Math.Max(order.SubTotal * (1 - foundCoupon.DiscountPercent), 0);
        }

        // Coupon not found or invalid → return subtotal
        return order.SubTotal;
    }


    // Mapper: Order -> OrderDto
    private OrderResponseDto MapToDto(Order order)
    {
        Status computedStatus = ComputeStatus(order);

        return new OrderResponseDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CreatedAt = order.CreatedAt,
            PickUpTime = order.PickUpTime,
            TotalPrice = CalculateTotal(order),
            Status = computedStatus,
            Drinks = order.Drinks.Select(d => new DrinkQuantity
            {
                DrinkName = d.DrinkName,
                Quantity = d.Quantity
            }).ToList(),
            Pizzas = order.Pizzas.Select(op => new PizzaInOrderDto
            {
                Id = op.Pizza.Id,
                Name = op.Pizza.Name,
                Price = op.Pizza.Price,
                IsGlutenFree = op.Pizza.IsGlutenFree,
                IsMenuPizza = op.Pizza.IsMenuPizza,
                Toppings = op.Pizza.Toppings,
                Quantity = op.Quantity
            }).ToList()
        };
    }

    //A mocked up method of having the order status change
    // if this was a real api this would probably calculate the status froma  table lookup or something
    private Status ComputeStatus(Order order)
    {
        var now = DateTime.UtcNow;
        if (order.PickUpTime.AddMinutes(10) < now)
            return Status.Delivered;
        if (order.PickUpTime < now)
            return Status.OnTheWay;
        return Status.Pending;
    }
}
