using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.Extensions.DependencyInjection;

public static class DbTestHelpers
{
    //Wipe the test database, call this at the top of every test
    public static async Task ResetDatabaseAsync(CustomWebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        await Task.CompletedTask;
    }

    //seed two test pizzas and return them in a list
    public static async Task<List<int>> SeedBasicPizzasAsync(
        CustomWebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();

        var pizzas = new List<Pizza>
    {
        new Pizza
        {
            Name = "Cheese Pizza",
            IsGlutenFree = false,
            IsMenuPizza = true,
            Price = 10.0,
            Toppings = new List<PizzaTopping>
            {
                new PizzaTopping { Topping = Topping.Cheese }
            }
        },
        new Pizza
        {
            Name = "Pepperoni Pizza",
            IsGlutenFree = false,
            IsMenuPizza = true,
            Price = 12.0,
            Toppings = new List<PizzaTopping>
            {
                new PizzaTopping { Topping = Topping.Pepperoni }
            }
        }
    };

        context.Pizzas.AddRange(pizzas);
        await context.SaveChangesAsync();

        return pizzas.Select(p => p.Id).ToList();
    }
}