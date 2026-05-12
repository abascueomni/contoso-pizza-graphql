using System.Net.Http.Json;
using System.Text.Json;
using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.Extensions.DependencyInjection;

public static class ApiTestHelper
{

    //function to create a test order, must be called after a pizza is inserted and passed a pizza id
    public static async Task<int> CreateOrderAsync(
        CustomWebApplicationFactory<Program> factory,
        int pizzaId)
    {
        var client = factory.CreateClient();

        var request = new
        {
            query = $@"
                mutation {{
                    createOrder(input: {{
                        customerName: ""Seed Customer {Guid.NewGuid()}"",
                        pizzas: [{{
                            pizzaId: {pizzaId},
                            quantity: 1
                        }}],
                        drinks: []
                    }}) {{
                        id
                    }}
                }}"
        };
        //send the request to gql
        var response = await client.PostAsJsonAsync("/gql", request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        //parse the response
        using var doc = JsonDocument.Parse(json);

        //return the created order id
        return doc.RootElement
            .GetProperty("data")
            .GetProperty("createOrder")
            .GetProperty("id")
            .GetInt32();
    }
}