namespace BullMarket.Application.Contracts;

public sealed record AddressDto(
    string RecipientName,
    string Line1,
    string? Line2,
    string City,
    string StateOrProvince,
    string PostalCode,
    string CountryCode,
    string? Phone);

public sealed record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    AddressDto? ShippingAddress,
    AddressDto? BillingAddress,
    DateTimeOffset CreatedAt);
