using EDAS.Common.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EDAS.Common.Services.Email;

public class LocalEmailService : IEmailService
{
    private readonly EmailConfig _emailConfig;
    private readonly HttpClient _httpClient;

    public LocalEmailService(IOptions<EmailConfig> emailConfigOptions,
        HttpClient httpClient)
    {
        _emailConfig = emailConfigOptions.Value;
        _httpClient = httpClient;
    }

    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        var emailContent = new EmailContent
        {
            From = new EmailAddress { Email = _emailConfig.FromEmail },
            To = new List<EmailAddress> { new EmailAddress { Email = emailRequest.Email } },
            Subject = emailRequest.Subject,
            Html = emailRequest.Message
        };

        var jsonContent = JsonSerializer.Serialize(emailContent);

        var request = new HttpRequestMessage(HttpMethod.Post, _emailConfig.ApiUrl)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", 
            _emailConfig.ApiKey);

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
