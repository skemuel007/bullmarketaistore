using FluentValidation;

namespace BullMarket.Api.Filters;

/// <summary>
/// Minimal API counterpart of <see cref="ValidationFilter"/>: validates the <typeparamref name="TRequest"/>
/// argument with its FluentValidation validator and returns a 400 validation problem on failure.
/// </summary>
public sealed class ValidationEndpointFilter<TRequest> : IEndpointFilter where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [typeof(TRequest).Name] = ["The request body is required."]
            });
        }

        var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
            if (!result.IsValid)
            {
                return TypedResults.ValidationProblem(result.ToDictionary());
            }
        }

        return await next(context);
    }
}

public static class ValidationEndpointFilterExtensions
{
    public static RouteHandlerBuilder WithValidation<TRequest>(this RouteHandlerBuilder builder)
        where TRequest : class =>
        builder.AddEndpointFilter<ValidationEndpointFilter<TRequest>>();
}
