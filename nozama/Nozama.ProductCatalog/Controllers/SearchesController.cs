using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Nozama.Model; // Make sure to import the appropriate namespace for your models
using Nozama.ProductCatalog.Data; 

namespace Nozama.ProductCatalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ProductCatalogDbContext _dbContext;

        public SearchController(ProductCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<Search>>> GetLatestSearches()
        {
            // Fetch the 100 latest searches ordered by search timestamp
            var latestSearches = await _dbContext.Searches
                .OrderByDescending(s => s.Timestamp)
                .Take(100)
                .ToListAsync();

            return Ok(latestSearches);
        }
    }
}
