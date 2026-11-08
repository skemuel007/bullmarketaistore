using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BullMarket.Api.Tests;

public class BullMarketApiFactory : WebApplicationFactory<Program>
{
    public Action<IServiceCollection>? ConfigureTestServicesAction { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:BullMarketAiStore"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["Database:ApplyMigrationsOnStartup"] = "false"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            ConfigureTestServicesAction?.Invoke(services);
        });
    }
}
