using EDAS.WebApp.Services.Database;

namespace EDAS.WebApp.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var databaseService = scope.ServiceProvider.GetService<IDatabaseService>();
            await databaseService.RunMigrationsAsync();
        }
    }

    public static async Task DeleteDatabase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var databaseService = scope.ServiceProvider.GetService<IDatabaseService>();
            await databaseService.DeleteDatabaseAsync();
        }
    }
}
