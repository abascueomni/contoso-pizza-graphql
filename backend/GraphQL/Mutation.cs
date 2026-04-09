using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Authorization;
using Microsoft.Extensions.Caching.Memory;
using ContosoPizza.Common;

namespace ContosoPizza.GraphQL;

public class Mutation
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<Mutation> _logger;
    public Mutation(IMemoryCache cache, ILogger<Mutation> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    [Authorize(Roles = new[] { "Admin" })]
    public async Task<Pizza> CreatePizza(
        CreatePizzaInput input,
        [Service] PizzaContext context)
    {
        var pizza = new Pizza
        {
            Name = input.Name,
            IsGlutenFree = input.IsGlutenFree,
            IsMenuPizza = input.IsMenuPizza,
            Price = CalculatePrice(input)
        };

        foreach (var topping in input.Toppings)
        {
            pizza.Toppings.Add(new PizzaTopping
            {
                Topping = topping
            });
        }
        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();
        //Invalidate the cache of all pizzas since we just added one
        _logger.LogDebug("Invalidating cache: {CacheKey}", CacheKeys.PizzasAll);
        _cache.Remove("pizzas_all");
        return pizza;
    }

    private double CalculatePrice(CreatePizzaInput pizzaInput)
    {
        double price = 5.0; // base price for a plain pizza

        if (pizzaInput.IsGlutenFree)
            price += 1.15;

        price += (pizzaInput.Toppings?.Count() ?? 0) * 0.75;

        return price;
    }
    [Authorize]
    public async Task<Order> CreateOrder(
        CreateOrderInput input,
        [Service] PizzaContext context)
    {
        var order = new Order
        {
            CustomerName = input.CustomerName,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var pizzaInput in input.Pizzas)
        {
            order.Pizzas.Add(new OrderPizza
            {
                PizzaId = pizzaInput.PizzaId,
                Quantity = pizzaInput.Quantity,
                Order = order
            });
        }
        foreach (var drinkInput in input.Drinks)
        {
            order.Drinks.Add(new DrinkQuantity
            {
                DrinkName = drinkInput.DrinkName,
                Quantity = drinkInput.Quantity,
            });
        }
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // Reload order with related data
        var createdOrder = await context.Orders
            .Include(o => o.Pizzas)
                .ThenInclude(op => op.Pizza)
                    .ThenInclude(p => p.Toppings)
            .Include(o => o.Drinks)
            .FirstAsync(o => o.Id == order.Id);

        return createdOrder;
    }

    [Authorize(Roles = new[] { "Admin" })]
    public async Task<bool> DeleteOrder(
        int orderId,
        [Service] PizzaContext context
    )
    {
        var order = await context.Orders.FindAsync(orderId);
        if (order == null) return false;

        context.Orders.Remove(order);
        await context.SaveChangesAsync();
        return true;
    }
}