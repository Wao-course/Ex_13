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
  private readonly ILogger<CatalogController> _logger;
  public CatalogController(ProductCatalogDbContext dbContext, ILogger<CatalogController> logger) 
  {
    _dbContext = dbContext;
    _logger = logger;
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

      // Log the search query
      LogSearch(name);

      var products = await _dbContext.Products
          .AsNoTracking()
          .Where(p => p.Name.Contains(name))
          .ToListAsync();

      return Ok(products);
  }

  private void LogSearch(string searchTerm)
  {
      // logic to log the search query, such as writing to a log file, database, or external service
      _logger.LogInformation("Search query: {searchTerm}", searchTerm);
  }
}
