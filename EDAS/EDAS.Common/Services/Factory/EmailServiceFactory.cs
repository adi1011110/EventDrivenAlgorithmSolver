using EDAS.Common.Services.Email;
using EDAS.Common.StaticDetails;
using Microsoft.Extensions.DependencyInjection;

namespace EDAS.Common.Services.Factory;

public static class EmailServiceFactory
{
    public static IEmailService Create(IServiceProvider serviceProvider, string environment)
    {
        return environment switch
        {
            EnvironmentConstants.DEVELOPMENT or EnvironmentConstants.DOCKER
                => serviceProvider.GetRequiredService<LocalEmailService>(),

            EnvironmentConstants.AZURE
                => serviceProvider.GetRequiredService<AzureEmailService>(),

            _ => throw new InvalidOperationException("Unknown environment")
        };
    }
}
