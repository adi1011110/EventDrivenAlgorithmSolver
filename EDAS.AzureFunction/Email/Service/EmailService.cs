using EDAS.AzureFunction.Email.Model;
using Microsoft.Extensions.Options;

namespace EDAS.AzureFunction.Email.EmailService;

public class EmailService : IEmailService
{
    private readonly EmailConfig _emailConfig;
    private readonly HttpClient _httpClient;

    public EmailService(IOptions<EmailConfig> emailConfigOptions,
        HttpClient httpClient)
    {
        _emailConfig = emailConfigOptions.Value;
        _httpClient = httpClient;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailContent = new
        {
            from = new { email = _emailConfig.FromEmail },
            to = new[] { new { email } },
            subject,
            html = htmlMessage
        };

        var jsonContent = JsonSerializer.Serialize(emailContent);

        var request = new HttpRequestMessage(HttpMethod.Post, _emailConfig.ApiUrl)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _emailConfig.ApiKey);

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
        }
    }
}