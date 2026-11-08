using BullMarket.Application.Abstractions;
using BullMarket.Domain.Cart;
using Microsoft.EntityFrameworkCore;

namespace BullMarket.Infrastructure.Persistence.Repositories;

internal sealed class CartRepository(BulkMarketAiStoreDbContext dbContext) : ICartRepository
{
    public Task<Cart?> GetByCustomerIdAsync(Guid customerId, bool trackChanges, CancellationToken cancellationToken)
    {
        var query = dbContext.Carts.Include(cart => cart.Items).AsQueryable();
        if (!trackChanges) query = query.AsNoTracking();
        return query.SingleOrDefaultAsync(cart => cart.CustomerId == customerId, cancellationToken);
    }

    public void Add(Cart cart) => dbContext.Carts.Add(cart);
}
