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
    public async Task<ActionResult<IEnumerable<StatsEntry>>> Get([FromQuery] long from = -1, [FromQuery] long to = -1)
    {
      DateTimeOffset f, t;
      if (from == -1)
      {
        f = DateTimeOffset.UtcNow.AddDays(-7);
      }
      else
      {
        f = DateTimeOffset.FromUnixTimeMilliseconds(from);
      }
      if (to == -1)
      {
        t = DateTimeOffset.UtcNow;
      }
      else
      {
        t = DateTimeOffset.FromUnixTimeMilliseconds(to);
      }
      return await _dbContext.Stats.Where(p => p.Timestamp.CompareTo(f) == 1 && p.Timestamp.CompareTo(t) == -1).Include(s => s.Products).ToListAsync();
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Recommendation>> Post(Recommendation recommendation)
    {
      await _dbContext.Recommendations.AddAsync(new Recommendation
      {
        Products = recommendation.Products,
        Timestamp = DateTimeOffset.Now,
      });
      await _dbContext.SaveChangesAsync();
      return Created($"{recommendation.RecommendationId}", recommendation);
    }


    [HttpGet("totallookups")]
    public ActionResult<int> GetTotalProductLookups()
    {
      Console.WriteLine("Getting total product lookups");
      try
      {
        var totalLookups = _productLookupService.CalculateTotalProductLookups();
        return Ok(totalLookups);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }
    }
  }
}
