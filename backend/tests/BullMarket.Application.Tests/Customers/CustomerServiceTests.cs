using BullMarket.Application.Abstractions;
using BullMarket.Application.Common;
using BullMarket.Application.Customers;
using BullMarket.Domain.Customers;
using FluentAssertions;
using NSubstitute;

namespace BullMarket.Application.Tests.Customers;

public class CustomerServiceTests
{
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(_customers);
    }

    [Fact]
    public async Task GetCustomerAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _customers.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var act = () => _sut.GetCustomerAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetCustomerAsync_WhenFound_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var customer = new Customer("John", "Doe", "john@example.com", "+123", DateTimeOffset.UtcNow);
        typeof(Customer).GetProperty("Id")!.SetValue(customer, id);

        _customers.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(customer);

        var result = await _sut.GetCustomerAsync(id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Email.Should().Be("john@example.com");
    }
}
