// Purpose: Background worker to process and save stats data from ProductCatalog to the Recommendations database.
// This worker will run in the background and periodically poll the ProductCatalog service for stats data.
// The stats data will be processed and saved to the Recommendations database.
// The worker will use the ProductCatalogService to poll the ProductCatalog service for stats data.
// The worker will use the RecommendationsDbContext to save the processed stats data to the database.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nozama.Model;
using Nozama.Recommendations;

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
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();

                    if (statsData != null)
                    {
                        foreach (var statsEntry in statsData)
                        {
                            if (statsEntry != null)
                            {
                                _logger.LogInformation("Processing stats data for term '{Term}'...", statsEntry.Term);
                                dbContext.Stats.Add(statsEntry); // Add StatsEntry to context

                                // Add each product in the StatsEntry to the context
                                foreach (var product in statsEntry.Products)
                                {
                                    await dbContext.Products.AddAsync(product);
                                }
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }

                _logger.LogInformation("Stats data processed and saved to database.");

                // Delay for 1600 milliseconds
                await Task.Delay(1600, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing or saving stats data to the database.");
            }
        }
    }



    // private StatsEntry ParseStatsData(IEnumerable<StatsEntry> statsData)
    // {
    //     // Initialize variables to hold extracted information
    //     List<Product> products = new List<Product>();
    //     string? term = null;
    //     DateTimeOffset timestamp = DateTimeOffset.MinValue;

    //     // Iterate through each StatsEntry object in the collection
    //     foreach (var statsEntry in statsData)
    //     {
    //         // Extract products from the current StatsEntry object
    //         if (statsEntry.Products != null)
    //         {
    //             products.AddRange(statsEntry.Products);
    //         }

    //         // Extract term from the current StatsEntry object
    //         if (!string.IsNullOrEmpty(statsEntry.Term))
    //         {
    //             term = statsEntry.Term;
    //         }

    //         // Extract timestamp from the current StatsEntry object
    //         if (statsEntry.Timestamp > timestamp)
    //         {
    //             timestamp = statsEntry.Timestamp;
    //         }
    //     }

    //     // Create a new StatsEntry object with the extracted information
    //     var parsedStatsEntry = new StatsEntry
    //     {
    //         Products = products,
    //         Term = term,
    //         Timestamp = timestamp
    //     };

    //     return parsedStatsEntry;
    // }

}
