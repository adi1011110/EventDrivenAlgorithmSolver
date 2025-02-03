var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        var builtConfig = config.Build();

        var keyVaultUrl = builtConfig["KeyVaultUri"];

        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            var keyVaultUri = new Uri(keyVaultUrl);

            var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());

            var emailConfigApiKey = secretClient.GetSecret(SecretNames.EmailConfig_ApiKey);
            var emailConfigFromEmail = secretClient.GetSecret(SecretNames.EmailConfig_FromEmail);
            var emailConfigApiUrl = secretClient.GetSecret(SecretNames.EmailConfig_ApiUrl);

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "EmailConfig:ApiKey", emailConfigApiKey.Value.Value},
                { "EmailConfig:FromEmail", emailConfigFromEmail.Value.Value},
                { "EmailConfig:ApiUrl", emailConfigApiUrl.Value.Value},
            });
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<EmailConfig>(context.Configuration.GetSection("EmailConfig"));
        services.AddHttpClient<IEmailService, EmailService>();
    })
    .Build();

host.Run();