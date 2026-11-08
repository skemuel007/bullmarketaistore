using BullMarket.Application.Catalog;

namespace BullMarket.Application.Abstractions;

public interface IProductRepository
{
    Task<(IReadOnlyCollection<Product> Items, int TotalCount)> SearchAsync(
        ProductSearchRequest request,
        CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(Guid id, bool trackChanges, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Category>> GetCategoriesAsync(CancellationToken cancellationToken);

    Task<string?> GetCategoryNameAsync(Guid categoryId, CancellationToken cancellationToken);
}
