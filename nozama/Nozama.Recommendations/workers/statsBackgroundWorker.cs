using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nozama.Model;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Add this using directive for JObject

public class StatsBackgroundWorker : BackgroundService
{
    private readonly ILogger<StatsBackgroundWorker> _logger;
    private readonly HttpClient _httpClient;
    private readonly RecommendationsDbContext _dbContext;

    public StatsBackgroundWorker(ILogger<StatsBackgroundWorker> logger, HttpClient httpClient, RecommendationsDbContext dbContext)
    {
        _logger = logger;
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Make GET request to /stats endpoint of product catalog service
                var response = await _httpClient.GetAsync("http://productcatalog/stats", stoppingToken);
                response.EnsureSuccessStatusCode();

                // Process stats data
                var statsData = await response.Content.ReadAsStringAsync();
                
                // Parse stats data and extract relevant information
                var statsEntry = ParseStatsData(statsData);

                // Save processed stats data to database
                _dbContext.Stats.Add(statsEntry);
                await _dbContext.SaveChangesAsync();

                // Delay for 1600 milliseconds
                await Task.Delay(1600, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while polling /stats from ProductCatalog.");
            }
        }
    }

    private StatsEntry ParseStatsData(string statsData)
    {
        // Deserialize the JSON string to extract relevant information
        var jsonObject = JObject.Parse(statsData);

        // Extract values for each property from the JSON object
        var data = jsonObject["data"]?.ToString();
        var products = jsonObject["products"]?.ToObject<List<Product>>()  ?? new List<Product>();
        var term = jsonObject["term"]?.ToString() ?? string.Empty;
        var timestamp = jsonObject["timestamp"]?.ToObject<DateTimeOffset>() ?? DateTimeOffset.UtcNow;

        // Create a new StatsEntry object with extracted values
        var statsEntry = new StatsEntry
        {
            Data = statsData, // Assuming statsData contains the raw JSON data
            Products = products,
            Term = term,
            Timestamp = DateTimeOffset.Now // Update this to use the correct timestamp, if available
        };


        return statsEntry;
    }
}
