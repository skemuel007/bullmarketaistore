using DomainCart = BullMarket.Domain.Cart.Cart;

namespace BullMarket.Application.Abstractions;

public interface ICartRepository
{
    Task<DomainCart?> GetByCustomerIdAsync(Guid customerId, bool trackChanges, CancellationToken cancellationToken);

    void Add(DomainCart cart);
}
