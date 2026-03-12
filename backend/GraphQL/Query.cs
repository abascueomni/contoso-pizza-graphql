using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Authorization;

public class Query
{
    [Authorize]
    public IQueryable<Pizza> GetPizzas([Service] PizzaContext context)
    {
        return context.Pizzas;
    }

    [Authorize]
    [UseFiltering]
    public IQueryable<Order> GetOrders([Service] PizzaContext context)
    {
        return context.Orders
            .Include(o => o.Pizzas)
            .ThenInclude(op => op.Pizza)
            .ThenInclude(t => t.Toppings)
            .Include(d => d.Drinks);
    }
}