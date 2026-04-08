using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Authorization;
using Microsoft.Extensions.Caching.Memory;
using ContosoPizza.Common;

namespace ContosoPizza.GraphQL;
public class Query
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<Query> _logger;
    public Query(IMemoryCache cache, ILogger<Query> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [Authorize]
    public async Task<List<Pizza>> GetPizzas([Service] PizzaContext context)
    {
        //Create a cachekey for all pizzas to return repeatedly named pizzas_all

        //if our cache already contains a value return it
        if (_cache.TryGetValue(CacheKeys.PizzasAll, out List<Pizza>? cached) && cached != null)
        {
            _logger.LogDebug("CACHE HIT: {CacheKey}", CacheKeys.PizzasAll);
            return cached;
        }
        _logger.LogDebug("CACHE MISS: {CacheKey}", CacheKeys.PizzasAll);
        //otherwise lookup the list of all pizzas with toppings
        var pizzas = await context.Pizzas
            .Include(p => p.Toppings)
            .ToListAsync();

        // Cache for 30 minutes since this is a low-change, high-read dataset,
        // reducing database hits while maintaining reasonable freshness
        _cache.Set(CacheKeys.PizzasAll, pizzas, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return pizzas;
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