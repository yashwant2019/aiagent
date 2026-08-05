using SampleApi.Models;

namespace SampleApi.Contracts;

/// <summary>
/// Product as returned by the API.
/// </summary>
public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    string Category,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt)
{
    public static ProductResponse FromProduct(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.Category,
        product.CreatedAt,
        product.UpdatedAt);
}
