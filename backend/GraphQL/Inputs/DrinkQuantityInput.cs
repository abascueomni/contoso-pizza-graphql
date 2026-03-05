using ContosoPizza.Models;

public class DrinkQuantityInput
{
    public Drink DrinkName { get; set; }  // enum
    public int Quantity { get; set; } = 1;
}