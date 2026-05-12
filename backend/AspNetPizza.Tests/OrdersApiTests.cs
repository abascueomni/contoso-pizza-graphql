using System.Net.Http.Json;
using System.Text.Json;

public class OrdersApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public OrdersApiTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ThenQuery_ReturnsOrder()
    {
        //reset the database before seeding it
        await DbTestHelpers.ResetDatabaseAsync(_factory);
        var pizzaIds = await DbTestHelpers.SeedBasicPizzasAsync(_factory);

        var createRequest = new
        {
            query = $@"
                mutation {{
                    createOrder(input: {{
                        customerName: ""Test Customer"",
                        drinks:  [{{
                            drinkName: AQUAFINA,
                            quantity: 1
                        }}],
                        pizzas:  [{{
                            pizzaId: {pizzaIds[0]},
                            quantity: 2
                        }}]
                    }}) {{
                        id
                        customerName
                    }}
                }}"
        };
        //submit the gql query
        var createOrderQueryResponse = await _client.PostAsJsonAsync("/gql", createRequest);
        //confirm the submission returned success
        createOrderQueryResponse.EnsureSuccessStatusCode();

        var createOrderJson = await createOrderQueryResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("errors", createOrderJson);

        using var createOrderDoc = JsonDocument.Parse(createOrderJson);
        //find the order id returned in response
        var orderId = createOrderDoc.RootElement
            .GetProperty("data")
            .GetProperty("createOrder")
            .GetProperty("id")
            .GetInt32();

        //use the found orderId to test get by id
        //form the gql query
        var queryRequest = new
        {
            query = $@"
                query {{
                    orders(where: {{ id: {{ eq: {orderId} }} }}) {{
                        id
                        customerName
                    }}
                }}
            "
        };
        //submit the gql query
        var queryResponse = await _client.PostAsJsonAsync("/gql", queryRequest);
        var queryJson = await queryResponse.Content.ReadAsStringAsync();
        //parse the result
        using var queryDoc = JsonDocument.Parse(queryJson);

        var orders = queryDoc.RootElement
            .GetProperty("data")
            .GetProperty("orders");
        //confirm we got a response for asking for the id we created earlier
        Assert.Equal(1, orders.GetArrayLength());
    }

    [Fact]
    public async Task SeedOrders_ThenQuery_ReturnsTwoOrders()
    {
        await DbTestHelpers.ResetDatabaseAsync(_factory);
        var pizzaIds = await DbTestHelpers.SeedBasicPizzasAsync(_factory);
        var orderId1 = await ApiTestHelper.CreateOrderAsync(_factory, pizzaIds[0]);
        Assert.True(orderId1 > 0);
        var orderId2 = await ApiTestHelper.CreateOrderAsync(_factory, pizzaIds[0]);
        Assert.True(orderId2 > 0);
        var getRequest = new
        {
            query = @"
              query {
                orders {
                    id
                }
            }
            "
        };
        //submit the gql query
        var queryResponse = await _client.PostAsJsonAsync("/gql", getRequest);
        var queryJson = await queryResponse.Content.ReadAsStringAsync();
        //parse the result
        using var queryDoc = JsonDocument.Parse(queryJson);

        var orders = queryDoc.RootElement
            .GetProperty("data")
            .GetProperty("orders");
        //confirm we got a response containing two orders
        Assert.Equal(2, orders.GetArrayLength());

        var ids = orders.EnumerateArray()
            .Select(o => o.GetProperty("id").GetInt32())
            .ToList();
        //Confirm each order we seeded is present
        Assert.Contains(orderId1, ids);
        Assert.Contains(orderId2, ids);
    }
    [Fact]
    public async Task DeleteOrder_ReturnsTrue()
    {
        await DbTestHelpers.ResetDatabaseAsync(_factory);
        var pizzaIds = await DbTestHelpers.SeedBasicPizzasAsync(_factory);
        var orderId = await ApiTestHelper.CreateOrderAsync(_factory, pizzaIds[0]);
        Assert.True(orderId > 0);
        var deleteRequest = new
        {
            query = $@"
                mutation {{
                    deleteOrder(orderId: {orderId})
                }}"
        };
        var deleteResponse = await _client.PostAsJsonAsync("/gql", deleteRequest);
        deleteResponse.EnsureSuccessStatusCode();

        var getRequest = new
        {
            query = @"
              query {
                orders {
                    id
                }
            }
            "
        };
        //submit the gql query
        var queryResponse = await _client.PostAsJsonAsync("/gql", getRequest);
        var queryJson = await queryResponse.Content.ReadAsStringAsync();
        //parse the result
        using var queryDoc = JsonDocument.Parse(queryJson);

        var orders = queryDoc.RootElement
            .GetProperty("data")
            .GetProperty("orders");
        //confirm we got a response containing zero orders
        Assert.Equal(0, orders.GetArrayLength());
    }
}