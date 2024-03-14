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
                .GroupBy(s => s.Term)
                .Select(g => new { Term = g.Key, Count = g.Count() }) // Calculate the count of elements in each group
                .OrderByDescending(g => g.Count) // Order by the count of elements in each group
                .Take(100)
                .ToListAsync();

            // Flatten the grouped results to get individual search records
            var flattenedSearches = latestSearches.SelectMany(g => _dbContext.Searches.Where(s => s.Term == g.Term)).ToList();

            return Ok(flattenedSearches);
        }

    }
}
