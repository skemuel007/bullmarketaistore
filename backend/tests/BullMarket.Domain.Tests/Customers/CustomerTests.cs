using BullMarket.Domain.Common;
using BullMarket.Domain.Customers;
using FluentAssertions;

namespace BullMarket.Domain.Tests.Customers;

public class CustomerTests
{
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void Constructor_WithValidData_NormalizesEmailAndInitializesCustomer()
    {
        var customer = new Customer("John", "Doe", "  JOHN.DOE@Example.com  ", "+1234567890", _now);

        customer.Id.Should().NotBeEmpty();
        customer.FirstName.Should().Be("John");
        customer.LastName.Should().Be("Doe");
        customer.Email.Should().Be("john.doe@example.com");
        customer.Phone.Should().Be("+1234567890");
        customer.CreatedAt.Should().Be(_now);
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ThrowsDomainException()
    {
        var act = () => new Customer("John", "Doe", "not-an-email", null, _now);

        act.Should().Throw<DomainException>()
            .WithMessage("*email*");
    }

    [Fact]
    public void SetShippingAndBillingAddress_AssignsAddressesCorrectly()
    {
        var customer = new Customer("Jane", "Smith", "jane@example.com", null, _now);
        var shipping = new Address("Jane Smith", "123 Main St", "Apt 4", "City", "State", "12345", "US", "+111");
        var billing = new Address("Jane Smith", "456 Market St", null, "City", "State", "12345", "US", "+111");

        customer.SetShippingAddress(shipping);
        customer.SetBillingAddress(billing);

        customer.ShippingAddress.Should().Be(shipping);
        customer.BillingAddress.Should().Be(billing);
    }
}
