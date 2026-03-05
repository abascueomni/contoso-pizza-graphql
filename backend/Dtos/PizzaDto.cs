using ContosoPizza.Models;

namespace ContosoPizza.DTOs;

public class PizzaDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsGlutenFree { get; set; }
    public bool IsMenuPizza { get; set; }
    public double Price { get; set; }
    public List<Topping> Toppings { get; set; } = new();
    public bool IsDeleted { get; set; }
}
