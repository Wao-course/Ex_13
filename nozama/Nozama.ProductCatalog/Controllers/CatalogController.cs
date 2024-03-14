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

      try 
      {
          // Log the search query
          await LogSearch(name);
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Failed to log search term");
      }
      
      var products = await _dbContext.Products
          .AsNoTracking()
          .Where(p => p.Name.Contains(name))
          .ToListAsync();

      return Ok(products);
  }

  public async Task LogSearch(string searchTerm)
  {
      // Create a new Search object with the search term and timestamp
      var search = new Search
      {
          Term = searchTerm,
          Timestamp = DateTimeOffset.Now
      };

      // Add the search object to the database context
      _dbContext.Searches.Add(search);

      // Save changes to the database asynchronously
      await _dbContext.SaveChangesAsync();
  }
}
