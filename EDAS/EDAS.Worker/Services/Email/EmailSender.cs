namespace EDAS.Worker.Services.Email;

public class EmailSender : EmailBaseService, IEmailSender
{
    public EmailSender(IOptions<EmailConfig> emailOptions) : base(emailOptions.Value)
    {
    }
}
