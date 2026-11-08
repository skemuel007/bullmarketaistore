using BullMarket.Application.Catalog;

namespace BullMarket.Application.Abstractions;

public interface IProductService
{
    Task<ProductSearchResult> GetProductsAsync(ProductSearchRequest request, CancellationToken cancellationToken);

    Task<ProductDetailsDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
}
