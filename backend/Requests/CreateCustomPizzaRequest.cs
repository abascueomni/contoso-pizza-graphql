using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ContosoPizza.Models;
namespace ContosoPizza.DTOs;

public class CreateCustomPizzaRequest
{
    public string? Name { get; set; }

    [DefaultValue(false)]
    public bool IsGlutenFree { get; set; } = false;
    public IEnumerable<Topping>? Toppings { get; set; }

    // Dropdown in Swagger for menu presets
    [EnumDataType(typeof(MenuPreset))]
    public MenuPreset MenuPreset { get; set; } = MenuPreset.None;
}