using BullMarket.Domain.Common;
using BullMarket.Domain.Customers;

namespace BullMarket.Domain.Orders;

public sealed class Order : IHasDomainEvents
{
    private readonly List<OrderItem> _items = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    private Order() {}

    public static Order Create(
        string orderNumber,
        Guid customerId,
        IEnumerable<OrderLine> lines,
        Address shippingAddress,
        Address billingAddress,
        decimal shippingAmount,
        decimal taxAmount,
        string currency,
        DateTimeOffset orderDate
    )
    {
        var orderLines = lines?.ToArray() ?? throw new ArgumentNullException(nameof(lines));

        if (orderLines.Length == 0)
        {
            throw new DomainException("Order must have at least one line item.", nameof(lines));
        }

        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer ID is required.", nameof(customerId));
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = Guard.Required(orderNumber, nameof(orderNumber), 50),
            CustomerId = customerId,
            ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress)),
            BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress)),
            ShippingAmount = Guard.NonNegative(shippingAmount, nameof(shippingAmount)),
            TaxAmount = Guard.NonNegative(taxAmount, nameof(taxAmount)),
            Currency = Guard.Required(currency, nameof(currency), 3),
            OrderDate = orderDate,
            OrderStatus = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending
        };

        foreach (var line in orderLines)
        {
            var orderItem = new OrderItem(
                order.Id,
                line
            );

            order._items.Add(orderItem);
        }

        order._domainEvents.Add(new OrderCreatedDomainEvent(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.TotalAmount,
            orderDate));

        return order;
    }

    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public DateTimeOffset OrderDate { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public Address BillingAddress { get; private set; } = null!;
    public decimal Subtotal => _items.Sum(item => item.LineTotal);
    public decimal ShippingAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount => Subtotal + ShippingAmount + TaxAmount;
    public string Currency { get; private set; } = string.Empty;
    public OrderStatus OrderStatus { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public uint Version { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void Confirm() => TransitionTo(OrderStatus.Confirmed, OrderStatus.Pending);
    public void StartProcessing() => TransitionTo(OrderStatus.Processing, OrderStatus.Confirmed);
    public void MarkShipped() => TransitionTo(OrderStatus.Shipped, OrderStatus.Processing);
    public void MarkDelivered() => TransitionTo(OrderStatus.Delivered, OrderStatus.Shipped);
    
    public void MarkReturned()
    {
        if (OrderStatus != OrderStatus.Delivered)
        {
            throw new DomainException($"Cannot mark order as returned when it is in '{OrderStatus}' status.", nameof(OrderStatus));
        }

        OrderStatus = OrderStatus.Returned;
    }

    public void Cancel(DateTimeOffset cancelledAt)
    {
        if (OrderStatus == OrderStatus.Cancelled)
        {
            throw new DomainException("Order is already cancelled.", nameof(OrderStatus));
        }

        if (OrderStatus == OrderStatus.Delivered || OrderStatus == OrderStatus.Returned)
        {
            throw new DomainException($"Cannot cancel order when it is in '{OrderStatus}' status.", nameof(OrderStatus));
        }

        OrderStatus = OrderStatus.Cancelled;
        CancelledAt = cancelledAt;
        _domainEvents.Add(new OrderCancelledDomainEvent(Id, CustomerId, cancelledAt));
    }

    public void SetPaymentStatus(PaymentStatus paymentStatus)
    {
        if( paymentStatus == PaymentStatus.Refunded && PaymentStatus != PaymentStatus.Paid)
        {
            throw new DomainException("Cannot mark order as refunded when it is not in 'Paid' status.", nameof(PaymentStatus));
        }

        PaymentStatus = paymentStatus;
    }

    private void TransitionTo(OrderStatus next, OrderStatus requiredCurrent)
    {
        if (OrderStatus != requiredCurrent)
        {
            throw new DomainException($"Order must be in {requiredCurrent} before it can become {next}.");
        }

        OrderStatus = next;
    }
}
