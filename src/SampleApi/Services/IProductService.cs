using SampleApi.Contracts;
using SampleApi.Models;

namespace SampleApi.Services;

public interface IProductService
{
    IReadOnlyList<Product> GetAll(string? category = null);

    Product? GetById(int id);

    Product Create(ProductRequest request);

    Product? Update(int id, ProductRequest request);

    bool Delete(int id);
}
