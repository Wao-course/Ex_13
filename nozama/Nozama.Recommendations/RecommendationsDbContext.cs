using Microsoft.EntityFrameworkCore;
using Nozama.Model;

public class RecommendationsDbContext : DbContext {
  public RecommendationsDbContext(DbContextOptions<RecommendationsDbContext> options) : base(options)
  {

  }

  public DbSet<Recommendation> Recommendations => Set<Recommendation>();
  // Inside RecommendationsDbContext class
  public DbSet<StatsEntry> Stats => Set<StatsEntry>();
  public DbSet<Search> Searches => Set<Search>();


}