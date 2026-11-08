using BullMarket.Domain.Cart;
using BullMarket.Domain.Customers;
using BullMarket.Domain.Inventory;
using BullMarket.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace BullMarket.Infrastructure;

public sealed class BulkMarketAiStoreDbContext(DbContextOptions<BulkMarketAiStoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BulkMarketAiStoreDbContext).Assembly);
        
        if (Database.IsNpgsql())
        {
            modelBuilder.Entity<InventoryItem>()
                .Property(item => item.Version).IsRowVersion();
            modelBuilder.Entity<Order>()
                .Property(order => order.Version).IsRowVersion();
        }
        else
        {
            modelBuilder.Entity<InventoryItem>()
                .Property(item => item.Version).IsConcurrencyToken();
            modelBuilder.Entity<Order>()
                .Property(order => order.Version).IsConcurrencyToken();
        }
        
        base.OnModelCreating(modelBuilder);
    }
    
    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction is not null)
        {
            return await action(cancellationToken);
        }
        
        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>  
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            var result = await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        });
    }
}
