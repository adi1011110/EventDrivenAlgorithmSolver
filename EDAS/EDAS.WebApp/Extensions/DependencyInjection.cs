using EDAS.Common.Services.RabbitMQ;
using EDAS.Common.StaticDetails;
using EDAS.WebApp.Services.Database;
using System.Reflection.PortableExecutable;

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
            options.SignIn.RequireConfirmedAccount = true;
        }).AddEntityFrameworkStores<EDASWebAppContext>();

        appBuilder.Services.AddAuthentication();

        appBuilder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
        });

        appBuilder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Home/Welcome"; 
            options.AccessDeniedPath = "/Home/Welcome";
        });

        appBuilder.Services.AddAutoMapper(typeof(MapperProfile));

        var rabbitMqConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rabbitmqconfig.json");

        appBuilder.Configuration.AddJsonFile(rabbitMqConfigPath, optional: false, reloadOnChange: true);

        appBuilder.Services.Configure<BrokerConfig>(appBuilder.Configuration.GetSection("RabbitMqConfig:Broker"));

        var queuesDict = appBuilder.Configuration
                                .GetSection("RabbitMqConfig:Queues")
                                .Get<Dictionary<string, QueueConfig>>();

        var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

        RegisterEnvironmentSpecificServices(appBuilder, EnvironmentUtils.GetEnvironmentVariable());

        appBuilder.Services.AddScoped<IProducerFactory, ProducerFactory>();

        appBuilder.Services.AddScoped<IDatabaseService, DatabaseService>();

        appBuilder.Services.AddSingleton(queuesConfigCollection);

        appBuilder.Services.AddHttpClient();     
    }

    private static void RegisterEnvironmentSpecificServices(WebApplicationBuilder builder, string env)
    {
        switch (env)
        {
            case EnvironmentConstants.AZURE:
                builder.Services.AddSingleton<IRabbitMQClientService, 
                    RabbitMQClientService<RabbitMQAzureConfig>>();
                builder.Services.AddScoped<IEmailService, AzureEmailService>();
                break;

            case EnvironmentConstants.DEVELOPMENT:
            case EnvironmentConstants.DOCKER:
                builder.Services.AddSingleton<IRabbitMQClientService, 
                    RabbitMQClientService<RabbitMQLocalConfig>>();
                builder.Services.AddScoped<IEmailService, LocalEmailService>();
                break;

            default:
                throw new InvalidOperationException($"Unknown environment for RabbitMQ: {env}");
        }

    }
}
