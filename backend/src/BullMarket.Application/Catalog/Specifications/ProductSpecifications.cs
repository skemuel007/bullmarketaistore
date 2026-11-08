using System.Linq.Expressions;
using BullMarket.Application.Common.Specifications;

namespace BullMarket.Application.Catalog.Specifications;

public sealed class ActiveProductSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression() =>
        product => product.IsActive;
}

public sealed class ProductPriceRangeSpecification(decimal? minPrice, decimal? maxPrice) : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression() =>
        product => (!minPrice.HasValue || product.Price >= minPrice.Value) &&
                   (!maxPrice.HasValue || product.Price <= maxPrice.Value);
}

public sealed class ProductHardwareSpecification(int? minRamGb, int? minVramGb) : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression() =>
        product => (!minRamGb.HasValue || product.RamGb >= minRamGb.Value) &&
                   (!minVramGb.HasValue || product.VramGb >= minVramGb.Value);
}

public sealed class ProductCapabilitiesSpecification(
    bool? aiReady,
    bool? localLlmReady,
    bool? developerReady,
    bool? creatorReady,
    bool? gamingReady) : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression() =>
        product => (!aiReady.HasValue || product.IsAiReady == aiReady.Value) &&
                   (!localLlmReady.HasValue || product.IsLocalLlmReady == localLlmReady.Value) &&
                   (!developerReady.HasValue || product.IsDeveloperReady == developerReady.Value) &&
                   (!creatorReady.HasValue || product.IsCreatorReady == creatorReady.Value) &&
                   (!gamingReady.HasValue || product.IsGamingReady == gamingReady.Value);
}
