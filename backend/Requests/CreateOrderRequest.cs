using ContosoPizza.Models;
namespace ContosoPizza.DTOs
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; } = "";

        // Pizza IDs + quantity
        public List<PizzaQuantity> Pizzas { get; set; } = new();
        public List<DrinkQuantity> Drinks { get; set; } = new();

        public string? Coupon { get; set; }
    }
}
