using System.Collections.Concurrent;
using SampleApi.Contracts;
using SampleApi.Models;

namespace SampleApi.Services;

/// <summary>
/// Stores products in memory. Registered as a singleton, so the data lives for
/// as long as the process does — swap this for an EF Core backed implementation
/// to persist it.
/// </summary>
public class InMemoryProductService : IProductService
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private readonly TimeProvider _timeProvider;
    private int _lastId;

    public InMemoryProductService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        Seed();
    }

    public IReadOnlyList<Product> GetAll(string? category = null) =>
        _products.Values
            .Where(p => category is null || p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Id)
            .ToList();

    public Product? GetById(int id) => _products.TryGetValue(id, out var product) ? product : null;

    public Product Create(ProductRequest request)
    {
        var product = new Product
        {
            Id = Interlocked.Increment(ref _lastId),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            CreatedAt = _timeProvider.GetUtcNow(),
        };

        _products[product.Id] = product;
        return product;
    }

    public Product? Update(int id, ProductRequest request)
    {
        if (!_products.TryGetValue(id, out var existing))
        {
            return null;
        }

        var updated = new Product
        {
            Id = existing.Id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = _timeProvider.GetUtcNow(),
        };

        // Only write if nobody changed the product while we were building the replacement.
        return _products.TryUpdate(id, updated, existing) ? updated : _products.GetValueOrDefault(id);
    }

    public bool Delete(int id) => _products.TryRemove(id, out _);

    private void Seed()
    {
        Create(new ProductRequest
        {
            Name = "Mechanical Keyboard",
            Description = "87-key hot-swappable board with brown switches.",
            Price = 89.99m,
            Category = "Peripherals",
        });

        Create(new ProductRequest
        {
            Name = "27\" 4K Monitor",
            Description = "IPS panel, 60 Hz, USB-C power delivery.",
            Price = 349.00m,
            Category = "Displays",
        });

        Create(new ProductRequest
        {
            Name = "Noise Cancelling Headphones",
            Price = 199.50m,
            Category = "Audio",
        });
    }
}
