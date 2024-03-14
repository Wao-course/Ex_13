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

        [HttpGet("latestSearches")]
        public async Task<ActionResult<IEnumerable<Search>>> GetLatestSearches()
        {
            var latestSearches = await _dbContext.Searches
                .GroupBy(s => s.Term) // Group by search term
                .OrderByDescending(g => g.Count()) // Order by the count of each term (i.e., number of occurrences)
                .Take(100) // Take the top 100 terms
                .SelectMany(g => g) // Flatten the grouped results to get individual search records
                .ToListAsync();

            return Ok(latestSearches);
        }
    }
}
