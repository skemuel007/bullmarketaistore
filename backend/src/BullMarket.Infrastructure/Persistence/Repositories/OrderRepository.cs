using BullMarket.Application.Abstractions;
using BullMarket.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace BullMarket.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(BulkMarketAiStoreDbContext dbContext) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Orders.Include(order => order.Items).SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Order>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken) =>
        await dbContext.Orders.AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.CustomerId == customerId)
            .OrderByDescending(order => order.OrderDate)
            .ToArrayAsync(cancellationToken);

    public void Add(Order order) => dbContext.Orders.Add(order);
}
