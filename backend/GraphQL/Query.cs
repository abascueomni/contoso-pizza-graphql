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
    private static int _hits = 0;
    private static int _misses = 0;
    //Calculate the cache hit rate
    private static double HitRate =>
        (_hits + _misses) == 0 ? 0 : (_hits * 100.0) / (_hits + _misses);

    public Query(IMemoryCache cache, ILogger<Query> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [Authorize]
    public async Task<List<Pizza>> GetPizzas([Service] PizzaContext context)
    {
        //if our cache already contains a value return it
        if (_cache.TryGetValue(CacheKeys.PizzasAll, out List<Pizza>? cached) && cached != null)
        {
            _hits++;
            _logger.LogDebug("CACHE HIT: {CacheKey}", CacheKeys.PizzasAll);

            _logger.LogInformation("Cache hit rate: {HitRate}", HitRate);

            return cached;
        }
        _misses++;
        _logger.LogDebug("CACHE MISS: {CacheKey}", CacheKeys.PizzasAll);
        _logger.LogInformation("Cache hit rate: {HitRate}", HitRate);

        //otherwise lookup the list of all pizzas with toppings
        var pizzas = await context.Pizzas
            .Include(p => p.Toppings)
            .ToListAsync();

        // Cache for 30 minutes since this is a low-change, high-read dataset,
        // reducing database hits while maintaining reasonable freshness
        // additionally add a logging callback for eviction which logs eviction reason
        _cache.Set(CacheKeys.PizzasAll, pizzas, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            PostEvictionCallbacks =
            {
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (key,value,reason,state) =>
                    {
                        _logger.LogInformation("Cache entry {CacheKey} evicted due to {Reason}",
                        key,
                        reason);
                    }
                }
            }
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