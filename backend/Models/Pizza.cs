using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoPizza.Models;

//A menu pizza with presets
public class Pizza
{
    public int Id { get; set; }                // Primary key for EF Core
    public string Name { get; set; } = "";    // Name of pizza
    public bool IsGlutenFree { get; set; }    // Gluten-free flag
    public bool IsMenuPizza { get; set; } = false; // Menu vs custom
    public List<PizzaTopping> Toppings { get; set; } = new(); // Toppings

    // Computed property for convenience
    [NotMapped]
    public List<Topping> ToppingEnums => Toppings?.Select(pt => pt.Topping).ToList() ?? new List<Topping>();

    public double Price { get; set; }         // Price
    public bool IsDeleted { get; set; } = false; // Soft delete

    private double CalculatePrice()
    {
        double price = 5.00;
        if (!IsMenuPizza)
        {
            if (IsGlutenFree)
                price += 1.15;
            price += Toppings.Count * 0.75;
        }
        return price;
    }
    public Pizza()
    {
        Toppings = new List<PizzaTopping>();
    }

    public Pizza(
        bool isGlutenFree,
        IEnumerable<PizzaTopping>? toppings = null,
        string name = "Custom Pizza",
        bool isMenuPizza = false)
    {
        Name = name;
        IsGlutenFree = isGlutenFree;
        IsMenuPizza = isMenuPizza;
        Toppings = toppings?.ToList() ?? new List<PizzaTopping>();
        Price = CalculatePrice();

    }
}
