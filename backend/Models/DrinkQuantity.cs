namespace ContosoPizza.Models
{
    public class DrinkQuantity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public Drink DrinkName { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
