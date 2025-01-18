var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;

        if (env.IsDevelopment())
        {
            config.AddUserSecrets<Program>();
        }
        var rabbitMqConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
            "rabbitmqconfig.json");
        config.AddJsonFile(rabbitMqConfigPath, optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services
            .RegisterConfigs(configuration)
            .RegisterServices();
        
        services.AddHostedService<ConsumerService>();
    })
    .Build();

await host.RunAsync();