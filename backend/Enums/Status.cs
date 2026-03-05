using System.ComponentModel.DataAnnotations;
namespace ContosoPizza.Models;

// Statuses
public enum Status
{
    [Display(Name = "Pending")] Pending,
    [Display(Name = "On the Way")] OnTheWay,
    [Display(Name = "Delivered")] Delivered,
    [Display(Name = "Cancelled")] Cancelled,
}
