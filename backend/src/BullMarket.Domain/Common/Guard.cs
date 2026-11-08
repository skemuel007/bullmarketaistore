namespace BullMarket.Domain.Common;

internal static class Guard
{
    public static string Required(string value, string paramName, int maxLength = int.MaxValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{paramName} is required.", paramName);
        }

        var result = value.Trim();

        if (result.Length > maxLength)
        {
            throw new DomainException($"{paramName} cannot exceed {maxLength} characters.", paramName);
        }

        return result;
    }

    public static decimal NonNegative(decimal value, string paramName)
    {
        if (value < 0)
        {
            throw new DomainException($"{paramName} cannot be negative.", paramName);
        }

        return value;
    }

    public static int NonNegative(int value, string paramName)
    {
        if (value < 0)
        {
            throw new DomainException($"{paramName} cannot be negative.", paramName);
        }

        return value;
    }

    public static int Positive(int value, string paramName)
    {
        if (value <= 0)
        {
            throw new DomainException($"{paramName} must be greater than zero.", paramName);
        }

        return value;
    }
}