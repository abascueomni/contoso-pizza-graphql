using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContosoPizza.DTOs;
using ContosoPizza.Models;  // Assuming Pizza is in this namespace

namespace ContosoPizza.Models
{
    public class Order
    {
        public int Id { get; set; }                       // Primary key
        public string CustomerName { get; set; } = "";    // Customer
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime PickUpTime { get; set; }
        public List<OrderPizza> Pizzas { get; set; } = new();
        public List<DrinkQuantity> Drinks { get; set; } = new();
        public string? Coupon { get; set; }
        [NotMapped]
        public double SubTotal => Pizzas.Sum(op => op.Pizza.Price * op.Quantity) + Drinks.Sum(d => d.Quantity * 2.00);
    }
}
