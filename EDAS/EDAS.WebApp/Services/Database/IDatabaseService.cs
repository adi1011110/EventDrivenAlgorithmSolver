namespace EDAS.WebApp.Services.Database
{
    public interface IDatabaseService
    {
        Task RunMigrationsAsync();

        Task DeleteDatabaseAsync ();
    }
}
