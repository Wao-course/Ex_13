using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nozama.Model; // Adjust the namespace as per your project structure

namespace Nozama.Recommendations.Services
{
    public class ProductLookupService
    {
        private readonly RecommendationsDbContext _dbContext;

        public ProductLookupService(RecommendationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<string, int>> GetTotalLookupsForProducts()
        {
            var totalLookups = new Dictionary<string, int>();

            // Query the database to get the total number of lookups for each product
            var searches = await _dbContext.Searches.ToListAsync();

            foreach (var search in searches)
            {
                // Increment the lookup count for each product
                if (!totalLookups.ContainsKey(search.Term))
                {
                    totalLookups.Add(search.Term, 1);
                }
                else
                {
                    totalLookups[search.Term]++;
                }
            }

            return totalLookups;
        }

        // public async Task<Dictionary<int, int>> GetTotalLookupsForProducts()
        // {
        //     var totalLookups = new Dictionary<int, int>();

        //     // Query the database to get the total number of lookups for each product
        //     var products = await _dbContext.Products.ToListAsync();

        //     foreach (var product in products)
        //     {
        //         var totalLookupCount = await _dbContext.Searches
        //             .CountAsync(s => s.ProductId == product.ProductId);
                
        //         // Add the product ID and its total lookup count to the dictionary
        //         totalLookups.Add(product.ProductId, totalLookupCount);
        //     }

        //     return totalLookups;
        // }
    }
}
