using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EDAS.Common.StaticDetails;

namespace EDAS.WebApp.Extensions;

public static class ConfigureExtensions
{
    public static void RegisterConfigs(this WebApplicationBuilder appBuilder)
    {
        var environment = EnvironmentUtils.GetEnvironmentVariable();
        RegisterConfigsHelper(appBuilder, environment);
    }

    private static void RegisterAzureConfigs(this WebApplicationBuilder appBuilder)
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

        appBuilder.Services.Configure<AzureFunctionConfig>(
            appBuilder.Configuration.GetSection("EmailConfig"));

        appBuilder.Services.Configure<RabbitMQAzureConfig>(
            appBuilder.Configuration.GetSection("RabbitMq"));
    }

    private static void RegisterDockerConfigs(WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.Configure<RabbitMQLocalConfig>(
            appBuilder.Configuration.GetSection("RabbitMQConfig"));

        appBuilder.Services.Configure<EmailConfig>(appBuilder.Configuration.GetSection("EmailConfig"));
    }

    private static void RegisterLocalConfigs(WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.Configure<RabbitMQLocalConfig>(
            appBuilder.Configuration.GetSection("RabbitMQConfig"));

        appBuilder.Services.Configure<EmailConfig>(appBuilder.Configuration.GetSection("EmailConfig"));
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
