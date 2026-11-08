using BullMarket.Application.Contracts;

namespace BullMarket.Application.Abstractions;

public interface ICustomerService
{
    Task<CustomerDto> GetCustomerAsync(Guid id, CancellationToken cancellationToken);
}
