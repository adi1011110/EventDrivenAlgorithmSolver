using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EDAS.WebApp.StaticDetails;

namespace EDAS.WebApp.Extensions;

public static class ConfigureExtensions
{
    public static void RegisterAzureConfigs(this WebApplicationBuilder appBuilder)
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

        appBuilder.Services.Configure<EmailConfig>(appBuilder.Configuration.GetSection("EmailConfig"));
    }
}
