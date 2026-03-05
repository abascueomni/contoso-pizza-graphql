using System.Text.Json.Serialization;

namespace ContosoPizza.Models;

public class PizzaTopping
{
    public int Id { get; set; }
    public Topping Topping { get; set; }

    public int PizzaId { get; set; }
    [JsonIgnore]  // <--- ignore during serialization    
    public Pizza Pizza { get; set; }
}