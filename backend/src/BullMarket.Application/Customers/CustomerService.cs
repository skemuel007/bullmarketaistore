using BullMarket.Application.Abstractions;
using BullMarket.Application.Common;
using BullMarket.Application.Contracts;
using BullMarket.Domain.Customers;

namespace BullMarket.Application.Customers;

public sealed class CustomerService(ICustomerRepository customers) : ICustomerService
{
    public async Task<CustomerDto> GetCustomerAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), id);
        return customer.ToDto();
    }
}
