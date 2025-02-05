using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EDAS.Common.StaticDetails;

namespace EDAS.WebApp.Extensions;

public static class ConfigureExtensions
{
    public static void RegisterAzureConfigs(this WebApplicationBuilder appBuilder)
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

        appBuilder.Services.Configure<EmailConfig>(appBuilder.Configuration.GetSection("EmailConfig"));
    }
}
