namespace BullMarket.Domain.Common;

public sealed class DomainException : Exception
{
    public DomainException(string message, string paramName) : base(message)
    {
        ParamName = paramName;
    }

    public DomainException(string message) : base(message)
    {
        ParamName = string.Empty;
    }

    public string ParamName { get; }
}