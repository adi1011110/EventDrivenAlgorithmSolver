namespace EDAS.WebApp.Utils;

public static class EmailRequestBuilder
{
    public static EmailRequest BuildEmailRequest(string email, string subject, string htmlContent)
    {
        var request = new EmailRequest
        {
            Email = email,
            Subject = subject,
            Message = htmlContent
        };

        return request;
    }
}
