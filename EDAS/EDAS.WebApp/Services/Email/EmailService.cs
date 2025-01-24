using Microsoft.AspNetCore.Identity.UI.Services;

namespace EDAS.WebApp.Services.Email;

public class EmailService : EmailBaseService, IEmailSender
{
    public EmailService(IOptions<EmailConfig> emailOptions) : base(emailOptions.Value)
    {
        
    }
}
