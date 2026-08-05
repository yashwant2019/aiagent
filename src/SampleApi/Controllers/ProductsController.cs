using Microsoft.AspNetCore.Mvc;
using SampleApi.Contracts;
using SampleApi.Services;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService products, ILogger<ProductsController> logger)
    {
        _products = products;
        _logger = logger;
    }

    /// <summary>Lists every product, optionally filtered by category.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<ProductResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ProductResponse>> GetAll([FromQuery] string? category)
    {
        var results = _products.GetAll(category).Select(ProductResponse.FromProduct);
        return Ok(results);
    }

    /// <summary>Gets a single product by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ProductResponse> GetById(int id)
    {
        var product = _products.GetById(id);
        if (product is null)
        {
            return NotFoundProblem(id);
        }

        return Ok(ProductResponse.FromProduct(product));
    }

    /// <summary>Creates a product.</summary>
    [HttpPost]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ProductResponse> Create([FromBody] ProductRequest request)
    {
        var product = _products.Create(request);
        _logger.LogInformation("Created product {ProductId}", product.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ProductResponse.FromProduct(product));
    }

    /// <summary>Replaces an existing product.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ProductResponse> Update(int id, [FromBody] ProductRequest request)
    {
        var product = _products.Update(id, request);
        if (product is null)
        {
            return NotFoundProblem(id);
        }

        return Ok(ProductResponse.FromProduct(product));
    }

    /// <summary>Deletes a product.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (!_products.Delete(id))
        {
            return NotFoundProblem(id);
        }

        _logger.LogInformation("Deleted product {ProductId}", id);
        return NoContent();
    }

    private ObjectResult NotFoundProblem(int id) => Problem(
        title: "Product not found",
        detail: $"No product exists with id {id}.",
        statusCode: StatusCodes.Status404NotFound);
}
