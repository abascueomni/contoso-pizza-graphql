using ContosoPizza.Models;

namespace ContosoPizza.DTOs
{
    public class PizzaInOrderDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double Price { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsMenuPizza { get; set; }
        public List<PizzaTopping> Toppings { get; set; } = new();
        public int Quantity { get; set; } = 1; // new
    }
}
