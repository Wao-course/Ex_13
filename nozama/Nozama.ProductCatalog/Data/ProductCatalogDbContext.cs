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
  public DbSet<Search> Searches => Set<Search>();
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Define the relationship between Product and Recommendation
    modelBuilder.Entity<Product>()
        .HasMany(p => p.Recommendations)
        .WithMany(r => r.Products);

    // Define the relationship between StatsEntry and Product
    modelBuilder.Entity<StatsEntry>()
        .HasMany(s => s.Products)
        .WithMany(p => p.StatsEntries); // Many-to-many relationship
}



}