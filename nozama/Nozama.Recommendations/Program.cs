using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<ProductCatalogService>();
builder.Services.AddHostedService<LastestSearchesBackgroundWorker>();
builder.Services.AddHostedService<StatsBackgroundWorker>();
// builder.Services.AddDbContext<RecommendationsDbContext>(
//     options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"))
// );
builder.Services.AddDbContextFactory<RecommendationsDbContext>(
    options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING")));


var app = builder.Build();

// This approach should not be used in production. See https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying
using(var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<RecommendationsDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log the exception or handle it gracefully
        Console.WriteLine($"Error occurred during database migration: {ex.Message}");
    }
}

app.MapGet("/", () => "Main page of the Recommendations service");

app.Run();
