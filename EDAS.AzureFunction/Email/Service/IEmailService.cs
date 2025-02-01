namespace EDAS.AzureFunction.Email.EmailService;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}