using BullMarket.Domain.Customers;

namespace BullMarket.Application.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
