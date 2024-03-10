using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Nozama.ProductCatalog.Data;
using Nozama.Model;

namespace Nozama.ProductCatalog.Controllers;

[ApiController]
[Route("[controller]")]
public class StatsController : ControllerBase
{

  private readonly ProductCatalogDbContext _dbContext;
  public StatsController(ProductCatalogDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  [HttpGet]
  public async Task<ActionResult<Recommendation>> Get()
  {
    return await Task.FromResult(new Recommendation {
      Products = new List<Product>(),
      Timestamp = DateTimeOffset.Now,
    });
  }  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<Recommendation>> Post(Recommendation recommendation)
  {
    await _dbContext.Recommendations.AddAsync(new Recommendation {
      Products = recommendation.Products,
      Timestamp = DateTimeOffset.Now,
    });
    await _dbContext.SaveChangesAsync();
    return Created($"{recommendation.RecommendationId}", recommendation);
  }
}
