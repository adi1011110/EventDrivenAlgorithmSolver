using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EDAS.WebApp.Data;
using EDAS.WebApp.Areas.Identity.Data;
using EDAS.WebApp.Services;
using EDAS.Common;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EDASWebAppContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'EDASWebAppContextConnection' not found.");

builder.Services.AddDbContext<EDASWebAppContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<EDASWebAppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<EDASWebAppContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(sp =>
    new RabbitMQClientService(RabbitMQConfig.HOSTNAME, RabbitMQConfig.USERNAME, RabbitMQConfig.PASSWORD));

builder.Services.AddTransient<IProducerService, ProducerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();