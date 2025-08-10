using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DynamicConfig;

public static class DynamicConfigMigrationExtensions
{
    public static IServiceCollection AddDynamicConfig(
            this IServiceCollection services,
            string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
    opt
        .UseNpgsql(connectionString, b =>
        {
            b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            b.MigrationsHistoryTable("__EFMigrationsHistory", "public"); // or your schema
        })
        .ConfigureWarnings(w =>
        {
            // Configure warnings as needed
            // w.Log(RelationalEventId.PendingModelChangesWarning);
        })
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
);
        return services;
    }
    public static async Task MigrateDynamicConfigAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DynamicConfig.Migration");
        var ctx = sp.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Daha ayrıntılı EF logları (sadece devde)
            ctx.Database.SetCommandTimeout(TimeSpan.FromSeconds(90));
            logger.LogInformation("Connection: {cs}", ctx.Database.GetDbConnection().ConnectionString);

            // EF hangi assembly'den migration arıyor?
            var migAsm = ctx.GetService<IMigrationsAssembly>().Assembly.GetName().Name;
            logger.LogInformation("MigrationsAssembly = {Asm}", migAsm);

            // Uygula
            await ctx.Database.MigrateAsync();
            logger.LogInformation("Migrate OK");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migrate FAILED");
            Console.WriteLine(ex.ToString()); // tüm inner exception zincirini yaz
            throw;
        }
    }
}
