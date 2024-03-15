  using Microsoft.AspNetCore.Mvc;
  using Nozama.ProductCatalog.Services;
  using Nozama.ProductCatalog.Data;
  using Nozama.Model;
using Microsoft.EntityFrameworkCore;

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
      public async Task<ActionResult<IEnumerable<Recommendation>>> Get()
      {
          try
          {
              // Retrieve recommendations from the database
              var recommendations = await _dbContext.Recommendations
                  .Include(r => r.Products) // Include associated products
                  .ToListAsync();

              return recommendations;
          }
          catch (Exception ex)
          {
              // Log and handle any exceptions that occur
              return StatusCode(StatusCodes.Status500InternalServerError, "Error while retrieving recommendations.");
          }
      }

      [HttpPost]
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


      [HttpGet("totallookups")]
      public ActionResult<int> GetTotalProductLookups()
      {
        var totalLookups = _productLookupService.CalculateTotalProductLookups();
        return Ok(totalLookups);
      }
    }
  }
