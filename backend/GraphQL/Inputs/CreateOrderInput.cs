using ContosoPizza.Models;

public class CreateOrderInput
{
    public string CustomerName { get; set; } = "";    // Customer
    public List<OrderPizzaInput> Pizzas { get; set; } = new();
    public List<DrinkQuantityInput> Drinks { get; set; } = new();
    public string? Coupon { get; set; }
}