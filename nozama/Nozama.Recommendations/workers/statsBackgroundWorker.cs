using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nozama.Model;
using Nozama.Recommendations;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore; // Add this using directive for JObject

public class StatsBackgroundWorker : BackgroundService
{

    private readonly ProductCatalogService _service;

    private readonly ILogger<StatsBackgroundWorker> _logger;
    private readonly IDbContextFactory<RecommendationsDbContext>? _dbContextFactory;
    
    private readonly IServiceScopeFactory _scopeFactory;

    public StatsBackgroundWorker(ILogger<StatsBackgroundWorker> logger, ProductCatalogService service, IDbContextFactory<RecommendationsDbContext> dbContext, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _service = service;
        _dbContextFactory = dbContext;
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Process stats data
                var statsData = await _service.GetStats();

                // Parse stats data and extract relevant information
                var statsEntry = ParseStatsData(statsData);

                // Save processed stats data to database
                var dbContext = _dbContextFactory.CreateDbContext();
                dbContext.Stats.Add(statsEntry);
                await dbContext.SaveChangesAsync();

                // Delay for 1600 milliseconds
                await Task.Delay(1600, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while polling /stats from ProductCatalog.");
            }
        }
    }

    private StatsEntry ParseStatsData(IEnumerable<StatsEntry> statsData)
    {
        // Initialize variables to hold extracted information
        List<Product> products = new List<Product>();
        string? term = null;
        DateTimeOffset timestamp = DateTimeOffset.MinValue;

        // Iterate through each StatsEntry object in the collection
        foreach (var statsEntry in statsData)
        {
            // Extract products from the current StatsEntry object
            if (statsEntry.Products != null)
            {
                products.AddRange(statsEntry.Products);
            }

            // Extract term from the current StatsEntry object
            if (!string.IsNullOrEmpty(statsEntry.Term))
            {
                term = statsEntry.Term;
            }

            // Extract timestamp from the current StatsEntry object
            if (statsEntry.Timestamp > timestamp)
            {
                timestamp = statsEntry.Timestamp;
            }
        }

        // Create a new StatsEntry object with the extracted information
        var parsedStatsEntry = new StatsEntry
        {
            Products = products,
            Term = term,
            Timestamp = timestamp
        };

        return parsedStatsEntry;
    }



}
