using EDAS.Common.StaticDetails;

namespace EDAS.Worker.Extensions;

public static class ConfigureExtensions
{
    public static IServiceCollection RegisterConfigs(this IServiceCollection services,
    WebApplicationBuilder appBuilder)
    {
        var environment = EnvironmentUtils.GetEnvironmentVariable();
        RegisterConfigsHelper(appBuilder, environment);

        return services;
    }

    private static void RegisterConfigsHelper(WebApplicationBuilder appBuilder, string environment)
    {
        environment = environment.ToLower();

        switch (environment)
        {
            case EnvironmentConstants.DEVELOPMENT:
                RegisterLocalConfigs(appBuilder);
                break;
            case EnvironmentConstants.DOCKER:
                RegisterDockerConfigs(appBuilder);
                break;
            case EnvironmentConstants.AZURE:
                RegisterAzureConfigs(appBuilder);
                break;
            default:
                throw new InvalidOperationException("Unknown environment");
        }
    }

    private static void RegisterAzureConfigs(WebApplicationBuilder appBuilder)
    {
        var keyVaultName = appBuilder.Configuration["AzureConfig:KeyVaultName"];

        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");

        var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());

        var rabbitMqUrl = secretClient.GetSecret(SecretNames.RabbitMQ_Url);
        var emailAppFunctionUrl = secretClient.GetSecret(SecretNames.EmailAppFunction_Url);
        var emailAppFunctionKey = secretClient.GetSecret(SecretNames.EmailAppFunction_Key);

        appBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "RabbitMq:Url", rabbitMqUrl.Value.Value },
            { "EmailConfig:AppFunctionUrl", emailAppFunctionUrl.Value.Value },
            { "EmailConfig:AppFunctionKey", emailAppFunctionKey.Value.Value }
        });

        appBuilder.Services.Configure<AzureFunctionConfig>(appBuilder.Configuration.GetSection("EmailConfig"));

        appBuilder.Services.Configure<WorkerType>(appBuilder.Configuration.GetSection("WorkerType"));

        appBuilder.Services.Configure<BrokerConfig>(appBuilder.Configuration.GetSection("RabbitMqConfig:Broker"));

        var workerType = appBuilder.Configuration["WorkerType:Type"];

        var queuesDict = appBuilder.Configuration
            .GetSection("RabbitMqConfig:Queues")
            .Get<Dictionary<string, QueueConfig>>();

        var queueConfig = queuesDict[workerType];

        var brokerConfig = appBuilder.Configuration.GetSection("RabbitMqConfig:Broker").Get<BrokerConfig>();

        var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

        appBuilder.Services.AddSingleton(provider =>
        {
            return new RabbitMqConfig
            {
                ExchangeName = brokerConfig.ExchangeName,
                ExchangeType = brokerConfig.ExchangeType,
                QueueName = queueConfig.QueueName,
                RoutingKey = queueConfig.RoutingKey,
                QueueDurable = queueConfig.QueueDurable,
                QueueExclusive = queueConfig.QueueExclusive,
                QueueAutodelete = queueConfig.QueueAutodelete,
                AlgorithmType = queueConfig.AlgorithmType
            }; ;
        });

        var rabbitMQUri = appBuilder.Configuration["RabbitMq:Url"];

        appBuilder.Services.AddSingleton(queuesConfigCollection);
    }

    private static void RegisterDockerConfigs(WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.Configure<WorkerType>(appBuilder.Configuration.GetSection("WorkerType"));

        appBuilder.Services.Configure<BrokerConfig>(
            appBuilder.Configuration.GetSection("RabbitMqConfig:Broker"));

        var workerType = appBuilder.Configuration["WorkerType:Type"];

        var queuesDict = appBuilder.Configuration
            .GetSection("RabbitMqConfig:Queues")
            .Get<Dictionary<string, QueueConfig>>();

        var queueConfig = queuesDict[workerType];

        var brokerConfig = appBuilder.Configuration.GetSection("RabbitMqConfig:Broker").Get<BrokerConfig>();

        var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

        appBuilder.Services.AddSingleton(provider =>
        {
            return new RabbitMqConfig
            {
                ExchangeName = brokerConfig.ExchangeName,
                ExchangeType = brokerConfig.ExchangeType,
                QueueName = queueConfig.QueueName,
                RoutingKey = queueConfig.RoutingKey,
                QueueDurable = queueConfig.QueueDurable,
                QueueExclusive = queueConfig.QueueExclusive,
                QueueAutodelete = queueConfig.QueueAutodelete,
                AlgorithmType = queueConfig.AlgorithmType
            }; ;
        });

        appBuilder.Services.AddSingleton(queuesConfigCollection);

        appBuilder.Services.Configure<RabbitMQLocalConfig>(
            appBuilder.Configuration.GetSection("RabbitMQConfig"));

        appBuilder.Services.Configure<EmailConfig>(appBuilder.Configuration.GetSection("EmailConfig"));
    }

    private static void RegisterLocalConfigs(WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.Configure<WorkerType>(appBuilder.Configuration.GetSection("WorkerType"));

        appBuilder.Services.Configure<BrokerConfig>(
            appBuilder.Configuration.GetSection("RabbitMqConfig:Broker"));

        var workerType = appBuilder.Configuration["WorkerType:Type"];

        var queuesDict = appBuilder.Configuration
            .GetSection("RabbitMqConfig:Queues")
            .Get<Dictionary<string, QueueConfig>>();

        var queueConfig = queuesDict[workerType];

        var brokerConfig = appBuilder.Configuration.GetSection("RabbitMqConfig:Broker").Get<BrokerConfig>();

        var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

        appBuilder.Services.AddSingleton(provider =>
        {
            return new RabbitMqConfig
            {
                ExchangeName = brokerConfig.ExchangeName,
                ExchangeType = brokerConfig.ExchangeType,
                QueueName = queueConfig.QueueName,
                RoutingKey = queueConfig.RoutingKey,
                QueueDurable = queueConfig.QueueDurable,
                QueueExclusive = queueConfig.QueueExclusive,
                QueueAutodelete = queueConfig.QueueAutodelete,
                AlgorithmType = queueConfig.AlgorithmType
            }; ;
        });

        appBuilder.Services.AddSingleton(queuesConfigCollection);

        appBuilder.Services.Configure<RabbitMQLocalConfig>(
            appBuilder.Configuration.GetSection("RabbitMQConfig"));

        appBuilder.Services.Configure<EmailConfig>(appBuilder.Configuration.GetSection("EmailConfig"));
    }

    public static bool IsDockerEnv(this IHostEnvironment env)
    {
        bool result = false;
        if (env.EnvironmentName.ToLower() == EnvironmentConstants.DOCKER)
        {
            result = true;
        }

        return result;
    }
}
