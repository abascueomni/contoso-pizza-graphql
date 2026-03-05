using System;
using System.Collections.Generic;
using ContosoPizza.Models;

namespace ContosoPizza.DTOs.Orders.V2
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime PickUpTime { get; set; }
        public List<PizzaInOrderDto> Pizzas { get; set; } = new();
        public List<DrinkQuantity> Drinks { get; set; } = new();
        public double TotalPrice { get; set; }
        public Status Status { get; set; }
    }
}
