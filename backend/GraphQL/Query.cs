using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using ContosoPizza.Common;
using System.Text.Json;
using System.Threading;

namespace ContosoPizza.GraphQL;

public class Query
{
    private static readonly SemaphoreSlim _pizzasLock = new(1, 1);
    private readonly IDistributedCache _cache;
    private readonly ILogger<Query> _logger;
    private static int _hits = 0;
    private static int _misses = 0;

    //Calculate the cache hit rate
    private static double HitRate =>
        (_hits + _misses) == 0 ? 0 : (_hits * 100.0) / (_hits + _misses);

    public Query(IDistributedCache cache, ILogger<Query> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [Authorize]
    public async Task<List<Pizza>> GetPizzas([Service] PizzaContext context)
    {
        var cached = await _cache.GetStringAsync(CacheKeys.PizzasAll);

        //if our cache already contains a value return it
        if (cached != null)
        {
            Interlocked.Increment(ref _hits);
            _logger.LogDebug("CACHE HIT: {CacheKey}", CacheKeys.PizzasAll);
            _logger.LogInformation("Cache hit rate: {HitRate}", HitRate);

            var deserializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var deserializedPizzas =
                JsonSerializer.Deserialize<List<Pizza>>(cached, deserializerOptions);

            //null check the deserialized list
            if (deserializedPizzas != null)
            {
                return deserializedPizzas;
            }
            else
            {
                _logger.LogWarning("Cache data for {CacheKey} was corrupted.", CacheKeys.PizzasAll);
            }
        }

        Interlocked.Increment(ref _misses);
        _logger.LogDebug("CACHE MISS: {CacheKey}", CacheKeys.PizzasAll);
        _logger.LogInformation("Cache hit rate: {HitRate}", HitRate);

        await _pizzasLock.WaitAsync();
        try
        {
            // re-check cache after acquiring lock to prevent stampede rebuild
            var cachedAgain = await _cache.GetStringAsync(CacheKeys.PizzasAll);

            if (cachedAgain != null)
            {
                Interlocked.Increment(ref _hits);
                _logger.LogDebug("CACHE HIT (post-lock): {CacheKey}", CacheKeys.PizzasAll);
                _logger.LogInformation("Cache hit rate: {HitRate}", HitRate);

                var deserializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<Pizza>>(cachedAgain, deserializerOptions)!;
            }

            //otherwise lookup the list of all pizzas with toppings
            var pizzas = await context.Pizzas
                .Include(p => p.Toppings)
                .ToListAsync();

            //serialize the list of pizzas
            var serializedPizzas = JsonSerializer.Serialize(pizzas);

            // Cache for 30 minutes since this is a low-change, high-read dataset,
            // reducing database hits while maintaining reasonable freshness
            await _cache.SetStringAsync(
                CacheKeys.PizzasAll,
                serializedPizzas,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                }
            );

            return pizzas;
        }
        finally
        {
            //release the lock on the pizzas cache
            _pizzasLock.Release();
        }
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