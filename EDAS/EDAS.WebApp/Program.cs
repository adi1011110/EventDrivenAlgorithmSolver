var builder = WebApplication.CreateBuilder(args);

builder.RegisterConfigs();

builder.RegisterServices();

builder.WebHost.UseStaticWebAssets();

bool conversionResult = bool.TryParse(builder.Configuration["DatabaseSettings:DeleteOnStartup"],
    out bool deleteOnStartup);

bool databaseDeletionCondition = conversionResult && deleteOnStartup;

var app = builder.Build();


if (databaseDeletionCondition)
{
    app.DeleteDatabase().GetAwaiter().GetResult();
}

app.ApplyMigrations().GetAwaiter().GetResult();

if (!(app.Environment.IsDevelopment() || app.Environment.IsDockerEnv()))
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 403 ||
        context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.Redirect("/Home/Welcome");
    }
});

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();