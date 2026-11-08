using BullMarket.Application.Abstractions;
using BullMarket.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using BullMarket.Infrastructure.Persistence;
using BullMarket.Infrastructure.Persistence.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BullMarket.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "BullMarketAiStore";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? configuration.GetConnectionString("BullMarket")
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<BulkMarketAiStoreDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(BulkMarketAiStoreDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
            }));

        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<DevelopmentDataSeeder>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
