
using Microsoft.EntityFrameworkCore;

namespace EDAS.WebApp.Services.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly EDASWebAppContext _appContext;

        public DatabaseService(EDASWebAppContext context)
        {
            _appContext = context;
        }

        public async Task DeleteDatabaseAsync()
        {
            await _appContext.Database.EnsureDeletedAsync();
        }

        public async Task RunMigrationsAsync()
        {
            if (_appContext.Database.GetPendingMigrations().Count() > 0)
            {
                await _appContext.Database.MigrateAsync();
            }
        }
    }
}
