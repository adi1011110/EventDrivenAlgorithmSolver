using EDAS.Common.Models;

namespace EDAS.Common.Services.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest emailRequest);
}
