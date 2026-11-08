using BullMarket.Application.Abstractions;
using BullMarket.Application.Cart;
using BullMarket.Application.Catalog;
using BullMarket.Application.Common;
using BullMarket.Application.Configuration;
using BullMarket.Application.Customers;
using BullMarket.Application.Inventory;
using BullMarket.Application.Orders;
using BullMarket.Application.Orders.Strategies;
using BullMarket.Domain.Orders;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BullMarket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        CommerceOptions? commerceOptions = null)
    {
        commerceOptions ??= new CommerceOptions();
        commerceOptions.Validate();
        services.AddSingleton(commerceOptions);
        services.AddValidatorsFromAssemblyContaining<ProductService>();
        services.AddLogging();
        services.AddSingleton(TimeProvider.System);

        services.AddSingleton<IShippingStrategy, TieredShippingStrategy>();
        services.AddSingleton<ITaxStrategy, StandardTaxStrategy>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<OrderCreatedDomainEvent>, OrderCreatedEventHandler>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInventoryService, InventoryService>();
        return services;
    }
}
