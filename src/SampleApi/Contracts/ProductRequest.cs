using System.ComponentModel.DataAnnotations;

namespace SampleApi.Contracts;

/// <summary>
/// Payload used to create or replace a product.
/// </summary>
public class ProductRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;
}
