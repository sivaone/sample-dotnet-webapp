using Microsoft.AspNetCore.Mvc;
using sample_dotnet_webapp.Models;

namespace sample_dotnet_webapp.Controllers;

[ApiController]
[Route("/api/products")]
public class ProductController : ControllerBase
{
    private static readonly Dictionary<int, Product?> Products = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Products);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Product? product)
    {
        if (product == null)
        {
            return BadRequest(new { message = "Product payload is required." });
        }

        if (product.Id == 0)
        {
            var nextId = Products.Keys.DefaultIfEmpty(0).Max() + 1;
            product.Id = nextId;
        }

        var added = Products.TryAdd(product.Id, product);
        if (added)
        {
            // Form absolute URI and return Location header with 201 status
            var uri = new Uri($"{Request.Scheme}://{Request.Host}/api/products/{product.Id}");
            return Created(uri, product);
        }
        else
        {
            return Conflict(new { message = $"Product with id '{product.Id}' already exists." });
        }
    }

    [HttpGet]
    [Route("/api/products/{id}")]
    public IActionResult Get(int id)
    {
        var found = Products.TryGetValue(id, out var product);
        if (found)
        {
            return Ok(product);
        }
        else
        {
            return NotFound(new { message = $"Product with id '{id}' not found." });
        }
    }
}