using Nozama.ProductCatalog.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nozama.ProductCatalog.Services
{
    public class ProductLookupService
    {
        private readonly ProductCatalogDbContext _dbContext;

        public ProductLookupService(ProductCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CalculateTotalProductLookups()
        {
            //the logic to calculate total product lookups
            return await _dbContext.Stats.CountAsync();
        }
    }
}
