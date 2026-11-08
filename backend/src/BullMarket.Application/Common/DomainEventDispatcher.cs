using BullMarket.Application.Abstractions;
using BullMarket.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BullMarket.Application.Common;

public sealed class DomainEventDispatcher(
    IServiceProvider serviceProvider,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler is null) continue;

                try
                {
                    var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                    if (method is not null)
                    {
                        var task = (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;
                        await task;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling domain event {EventType}", eventType.Name);
                    throw;
                }
            }
        }
    }
}
