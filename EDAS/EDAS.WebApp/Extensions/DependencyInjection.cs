using EDAS.WebApp.Services.Database;

namespace EDAS.WebApp.Extensions;

public static class DependencyInjection
{
    public static void RegisterServices(this WebApplicationBuilder appBuilder)
    {
        var connectionString = appBuilder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        appBuilder.Services.AddDbContext<EDASWebAppContext>(options => options.UseSqlite(connectionString));

        appBuilder.Services.AddDefaultIdentity<EDASWebAppUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 10;
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.SignIn.RequireConfirmedEmail = true;
        }).AddEntityFrameworkStores<EDASWebAppContext>();

        appBuilder.Services.AddAuthentication();

        appBuilder.Services.AddControllersWithViews();

        appBuilder.Services.AddAutoMapper(typeof(MapperProfile));

        var rabbitMqConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rabbitmqconfig.json");

        appBuilder.Configuration.AddJsonFile(rabbitMqConfigPath, optional: false, reloadOnChange: true);

        appBuilder.Services.Configure<BrokerConfig>(appBuilder.Configuration.GetSection("RabbitMqConfig:Broker"));

        var queuesDict = appBuilder.Configuration
                                .GetSection("RabbitMqConfig:Queues")
                                .Get<Dictionary<string, QueueConfig>>();

        var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

        var rabbitMQUri = appBuilder.Configuration["RabbitMq:Url"];

        appBuilder.Services.AddSingleton(sp =>
            new RabbitMQClientService(rabbitMQUri));

        appBuilder.Services.AddScoped<IProducerFactory, ProducerFactory>();

        appBuilder.Services.AddScoped<IDatabaseService, DatabaseService>();

        appBuilder.Services.AddSingleton(queuesConfigCollection);

        //TO DO: Fix
        //appBuilder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailService>();
    }
}
