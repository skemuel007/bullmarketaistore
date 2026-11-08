using BullMarket.Application.Abstractions;

namespace BullMarket.Infrastructure.Persistence;

internal sealed class UnitOfWork(BulkMarketAiStoreDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken) =>
        dbContext.ExecuteInTransactionAsync(operation, cancellationToken);
}
