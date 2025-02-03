namespace EDAS.Worker;

public static class DependencyInjection
{
    public static IServiceCollection RegisterConfigs(this IServiceCollection services,
        WebApplicationBuilder appBuilder)
    {
        var keyVaultName = appBuilder.Configuration["AzureConfig:KeyVaultName"];

        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");

        var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());

        var rabbitMqUrl = secretClient.GetSecret(SecretNames.RabbitMQ_Url);
        var emailConfigApiKey = secretClient.GetSecret(SecretNames.EmailConfig_ApiKey);
        var emailConfigFromEmail = secretClient.GetSecret(SecretNames.EmailConfig_FromEmail);
        var emailConfigApiUrl = secretClient.GetSecret(SecretNames.EmailConfig_ApiUrl);

        appBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "RabbitMq:Url", rabbitMqUrl.Value.Value },
            { "EmailConfig:ApiKey", emailConfigApiKey.Value.Value},
            { "EmailConfig:FromEmail", emailConfigFromEmail.Value.Value},
            { "EmailConfig:ApiUrl", emailConfigApiUrl.Value.Value},
        });

        services.Configure<EmailConfig>(appBuilder.Configuration.GetSection("EmailConfig"));

        services.Configure<WorkerType>(appBuilder.Configuration.GetSection("WorkerType"));

        services.Configure<BrokerConfig>(appBuilder.Configuration.GetSection("RabbitMqConfig:Broker"));

        var workerType = appBuilder.Configuration["WorkerType:Type"];

        var queuesDict = appBuilder.Configuration
            .GetSection("RabbitMqConfig:Queues")
            .Get<Dictionary<string, QueueConfig>>();

        var queueConfig = queuesDict[workerType];

        var brokerConfig = appBuilder.Configuration.GetSection("RabbitMqConfig:Broker").Get<BrokerConfig>();

        var queuesConfigCollection = new QueueConfigCollection { QueuesConfig = queuesDict };

        services.AddSingleton(provider =>
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

        services.AddSingleton(sp =>
        {
            return new RabbitMQClientService(rabbitMQUri);
        });

        services.AddSingleton(queuesConfigCollection);

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ICombinationsAlgorithmFactory, CombinationsAlgorithmFactory>();

        services.AddScoped<ISortingAlgorithmFactory, SortingAlgorithmFactory>();

        services.AddScoped<IEmailSender, EmailSender>();

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

        return services;
    }
}
