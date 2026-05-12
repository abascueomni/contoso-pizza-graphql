using ContosoPizza.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private SqliteConnection _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //use Testing environment which disables default Authentication
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // remove existing DbContext registration
            var dbDescriptors = services
                .Where(d =>
                    d.ServiceType.FullName != null &&
                    d.ServiceType.FullName.Contains("PizzaContext"))
                .ToList();

            foreach (var d in dbDescriptors)
                services.Remove(d);
            // SQLite in-memory
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<PizzaContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // AUTH
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "Test", options => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
                    new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder("Test")
                        .RequireAuthenticatedUser()
                        .Build();
            });
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:Key"] = "test-key-that-is-long-enough-to-pass-validation"
            });
        });


    }
}