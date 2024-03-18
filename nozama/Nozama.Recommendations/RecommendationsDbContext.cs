using Microsoft.EntityFrameworkCore;
using Nozama.Model;

public class RecommendationsDbContext : DbContext
{
  public RecommendationsDbContext(DbContextOptions<RecommendationsDbContext> options) : base(options)
  {

  }
  public DbSet<Product> Products => Set<Product>();

  public DbSet<Recommendation> Recommendations => Set<Recommendation>();
  // Inside RecommendationsDbContext class
  public DbSet<StatsEntry> Stats { get; set; }
  public DbSet<Search> Searches { get; set; }
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