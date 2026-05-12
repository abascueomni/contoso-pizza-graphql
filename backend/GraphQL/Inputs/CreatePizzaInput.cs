using ContosoPizza.Models;

public class CreatePizzaInput
{
    public string Name { get; set; } = "";    // Name of pizza
    public bool IsGlutenFree { get; set; }    // Gluten-free flag
    public bool IsMenuPizza { get; set; } = false; // Menu vs custom
    public List<ToppingInput> Toppings { get; set; } = new(); // store toppings
    public double Price { get; set; }         // Price
}