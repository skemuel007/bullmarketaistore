using BullMarket.Domain.Common;

namespace BullMarket.Domain.Customers;

public sealed class Customer
{
    private Customer() {}

    public Customer(string firstName, string lastName, string email, string? phone, DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid();
        FirstName = Guard.Required(firstName, nameof(firstName), 100);
        LastName = Guard.Required(lastName, nameof(lastName), 100);
        Email = NormalizeEmail(email);
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }

    public Address? ShippingAddress { get; private set; }
    public Address? BillingAddress { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public void SetShippingAddress(Address address)
    {
        ShippingAddress = address ?? throw new ArgumentNullException(nameof(address));
    }

    public void SetBillingAddress(Address billingAddress)
    {
        BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
    }

    private static string NormalizeEmail(string email)
    {
        var normalized = Guard.Required(email, nameof(email), 320).Trim().ToLowerInvariant();

        try
        {
            var addr = new System.Net.Mail.MailAddress(normalized);
            return addr.Address;
        }
        catch (FormatException)
        {
            throw new DomainException("Invalid email format.", nameof(email));
        }
    }
}