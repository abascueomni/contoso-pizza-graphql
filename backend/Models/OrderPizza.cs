namespace ContosoPizza.Models
{
    public class OrderPizza
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int PizzaId { get; set; }
        public Pizza Pizza { get; set; } = null!;

        public int Quantity { get; set; } = 1; // number of this pizza in the order
    }
}
