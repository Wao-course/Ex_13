using Microsoft.AspNetCore.Mvc;
using Nozama.ProductCatalog.Services;
using Nozama.ProductCatalog.Data;
using Nozama.Model;

namespace Nozama.ProductCatalog.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class StatsController : ControllerBase
  {
    private readonly ProductLookupService _productLookupService;
    private readonly ProductCatalogDbContext _dbContext;

    public StatsController(ProductLookupService productLookupService, ProductCatalogDbContext dbContext)
    {
      _productLookupService = productLookupService;
      _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<Recommendation>> Get()
    {
      return await Task.FromResult(new Recommendation
      {
        Products = new List<Product>(),
        Timestamp = DateTimeOffset.Now,
      });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Recommendation>> Post(Recommendation recommendation)
    {
      try
      {
        // Add the received recommendation to the DbContext
        _dbContext.Recommendations.Add(recommendation);

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        // Return the created recommendation
        return CreatedAtAction(nameof(Get), new { id = recommendation.RecommendationId }, recommendation);
      }
      catch (Exception ex)
      {
        // Log any exceptions that occur during the saving process
        return StatusCode(StatusCodes.Status500InternalServerError, "Error while saving the recommendation.");
      }
    }

    [HttpGet("totallookups")]
    public ActionResult<int> GetTotalProductLookups()
    {
      var totalLookups = _productLookupService.CalculateTotalProductLookups();
      return Ok(totalLookups);
    }
  }
}
