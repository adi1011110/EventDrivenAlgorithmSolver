using EDAS.Common.Services.RabbitMQ;
using EDAS.Common.StaticDetails;
using Microsoft.Extensions.Options;
using System.Reflection.PortableExecutable;

namespace EDAS.Worker.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddScoped<ICombinationsAlgorithmFactory, CombinationsAlgorithmFactory>();

        services.AddScoped<ISortingAlgorithmFactory, SortingAlgorithmFactory>();

        services.AddScoped<IEmailService, AzureEmailService>();

        services.AddScoped<IQueueFactory, QueueFactory>();

        var assembly = typeof(Program).Assembly;

        //NOTE: MediatR scope is singleton in BackgroundService
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        services.AddScoped(typeof(IRequestHandler<CombinationsInputCommand, CombinationsOutput>),
            typeof(SolveCombinationsAlgorithmHandler));

        services.AddScoped(typeof(IRequestHandler<SortingInputCommand, SortingOutputResult>),
            typeof(SolveSortingAlgorithmHandler));

        services.AddAutoMapper(typeof(MapperProfile));

        RegisterEnvironmentSpecificServices(services, EnvironmentUtils.GetEnvironmentVariable());

        return services;
    }

    private static void RegisterEnvironmentSpecificServices(IServiceCollection services, string env)
    {
        switch (env)
        {
            case EnvironmentConstants.AZURE:
                services.AddSingleton<IRabbitMQClientService>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<RabbitMQAzureConfig>>();
                    return new RabbitMQClientService<RabbitMQAzureConfig>(options);
                });
                services.AddScoped<IEmailService, AzureEmailService>();
                break;

            case EnvironmentConstants.DEVELOPMENT:
            case EnvironmentConstants.DOCKER:
                services.AddSingleton<IRabbitMQClientService>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<RabbitMQLocalConfig>>();
                    return new RabbitMQClientService<RabbitMQLocalConfig>(options);
                });
                services.AddScoped<IEmailService, LocalEmailService>();
                break;

            default:
                throw new InvalidOperationException($"Unknown environment for RabbitMQ: {env}");
        }

    }
}
