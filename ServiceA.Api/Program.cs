using DynamicConfig;
using DynamicConfig.Repositories.Abstract;
using DynamicConfig.Repositories.Concrete;
using DynamicConfig.Services.Abstract;
using DynamicConfig.Services.Concrete;
using DynamicConfig.Web;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

var appName = builder.Configuration["DynamicConfig:ApplicationName"] ?? "SERVICE-A";
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
var refreshMs = int.Parse(builder.Configuration["DynamicConfig:RefreshTimerIntervalInMs"] ?? "30000");
if (string.IsNullOrWhiteSpace(connStr))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(connStr);

});

builder.Services.AddDynamicConfig(connStr);
builder.Services.AddSingleton<IConfigurationReader>(_ =>
    new ConfigurationReader(appName, connStr, refreshMs));
builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
builder.Services.AddScoped<IConfigService, ConfigService>();
// Add services to the container.
builder.Services
    .AddRazorPages(options =>
    {
        options.Conventions.AddAreaPageRoute(
            areaName: "MyFeature",
            pageName: "/Index",
            route: "dynamic-config/{app}"
        );
    })
    .AddApplicationPart(typeof(UiMarker).Assembly);



var app = builder.Build();
await app.MigrateDynamicConfigAsync();
// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Check if database exists and has tables
        if (!context.Database.CanConnect())
        {
            // Apply migrations only if database doesn't exist
            context.Database.Migrate();
            Console.WriteLine("Database migration completed successfully for SERVICE-A.");
        }
        else
        {
            Console.WriteLine("Database already exists, skipping migration for SERVICE-A.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed for SERVICE-A: {ex.Message}");
        throw;
    }
}

app.MapGet("/dynamic-config", () => Results.Redirect($"/dynamic-config/{appName}"));

app.MapGet($"/dynamic-config/{{rest:regex(^(?!{Regex.Escape(appName)}$).*)}}",
    (string rest) => Results.Redirect($"/dynamic-config/{appName}")
);

app.UseHttpsRedirection();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});
var jsonOpts = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.Never
};
app.MapGet("/cfg", () => Results.BadRequest("key is required"));
app.MapGet("/cfg/{key}", (IConfigurationReader reader, string key) =>
{
    var val = reader.GetValue<object>(key);
    var runtimeType = val?.GetType().Name;
    return Results.Json(new { key, value = val, runtimeType }, jsonOpts);
});

app.UseStaticFiles();
app.MapRazorPages();

app.Run();
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
