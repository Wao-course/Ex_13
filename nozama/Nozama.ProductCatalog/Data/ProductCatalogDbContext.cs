using Microsoft.EntityFrameworkCore;
using Nozama.Model;

namespace Nozama.ProductCatalog.Data;

public class ProductCatalogDbContext : DbContext
{
  public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : base(options)
  {

  }

  public DbSet<Product> Products => Set<Product>();
  public DbSet<Recommendation> Recommendations => Set<Recommendation>();
  public DbSet<StatsEntry> Stats => Set<StatsEntry>();
}