using System.Net.Http.Json;
using System.Text.Json;

public class PizzaApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PizzaApiTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task CreatePizza_ReturnsCreated()
    {
        //reset the database before seeding it
        await DbTestHelpers.ResetDatabaseAsync(_factory);
        //form the gql request to add a new pizza
        var request = new
        {
            query = @"
                mutation {
                    createPizza(input: {
                        name: ""Test Pizza"",
                        isGlutenFree: true,
                        isMenuPizza: false,
                        price: 12.99,
                        toppings: [
                            { topping: ""CHEESE"" },
                            { topping: ""PEPPERONI"" }
                        ]
                    }) {
                        name
                    }
                }
            "
        };
        //send the mutation
        var response = await _client.PostAsJsonAsync("/gql", request);
        //confirm it was a success
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        //confirm that the success contains the pizza name just added
        Assert.Contains("Test Pizza", json);
    }

    [Fact]
    public async Task GetAllPizzas_ReturnsTwo()
    {
        //reset the database before seeding it
        await DbTestHelpers.ResetDatabaseAsync(_factory);
        //seed basic pizzas
        await DbTestHelpers.SeedBasicPizzasAsync(_factory);
        //form the gql request for pizzas by name
        var request = new
        {
            query = @"
                query {
                    pizzas {
                        name
                    }
                }
            "
        };
        //send the query
        var response = await _client.PostAsJsonAsync("/gql", request);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        //get the pizzas array
        var pizzas = doc.RootElement
            .GetProperty("data")
            .GetProperty("pizzas");
        //assert that there are two pizzas
        Assert.Equal(2, pizzas.GetArrayLength());
    }
}