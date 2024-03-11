using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Nozama.ProductCatalog.Data;
using Nozama.Model;

namespace Nozama.ProductCatalog.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase 
{

  private readonly ProductCatalogDbContext _dbContext;

  public CatalogController(ProductCatalogDbContext dbContext) 
  {
    _dbContext = dbContext;
  }

  [HttpGet]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<IEnumerable<Product>>> Get() 
  {
    var products = await _dbContext.Products.AsNoTracking().ToListAsync();
    return Ok(products);
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<Product>> Post(Product product) {
    await _dbContext.Products.AddAsync(product);
    await _dbContext.SaveChangesAsync();
    return Created($"{product.ProductId}", product);
  }

[HttpGet("search")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<IEnumerable<Product>>> SearchByName([FromQuery] string name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return BadRequest("Search term cannot be empty.");
    }

    var products = await _dbContext.Products
        .AsNoTracking()
        .Where(p => p.Name.Contains(name))
        .ToListAsync();

    return Ok(products);
}

}