using System.ComponentModel.DataAnnotations;
namespace ContosoPizza.Models;

// Allowed toppings
public enum Topping
{
    [Display(Name = "Cheese")] Cheese,
    [Display(Name = "Tomato")] Tomato,
    [Display(Name = "Peppers")] Peppers,
    [Display(Name = "Mushrooms")] Mushrooms,
    [Display(Name = "Pepperoni")] Pepperoni,
    [Display(Name = "Bell Pepper Red")] BellPepperRed,
    [Display(Name = "Bell Pepper Green")] BellPepperGreen,
    [Display(Name = "Onion")] Onion,
    [Display(Name = "Sausage")] Sausage,
    [Display(Name = "Ham")] Ham,
    [Display(Name = "Pineapple")] Pineapple
}
