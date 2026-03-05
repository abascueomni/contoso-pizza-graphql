using System.ComponentModel.DataAnnotations;
namespace ContosoPizza.Models;

public enum Drink
{
    [Display(Name = "Pepsi")]
    Pepsi,

    [Display(Name = "Pepsi Zero Sugar")]
    PepsiZero,

    [Display(Name = "Mountain Dew")]
    MountainDew,

    [Display(Name = "Starry")]
    Starry,

    [Display(Name = "Mug Root Beer")]
    MugRootBeer,

    [Display(Name = "Dr Pepper")]
    DrPepper,

    [Display(Name = "Aquafina Water")]
    Aquafina
}
