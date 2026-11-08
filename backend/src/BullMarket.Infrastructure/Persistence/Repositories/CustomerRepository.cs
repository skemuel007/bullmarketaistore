using BullMarket.Application.Abstractions;
using BullMarket.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace BullMarket.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository(BulkMarketAiStoreDbContext dbContext) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Customers.AsNoTracking().SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);
}
