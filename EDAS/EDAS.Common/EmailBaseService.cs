using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using EDAS.Common.Models;
using Newtonsoft.Json;

namespace EDAS.Common;

public abstract class EmailBaseService
{
    protected readonly EmailConfig _emailConfig;

    public EmailBaseService(EmailConfig emailConfig)
    {
        _emailConfig = emailConfig;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new HttpClient();

        var emailContent = new
        {
            from = new { email = _emailConfig.FromEmail },
            to = new[] { new { email = email } },
            subject = subject,
            html = htmlMessage
        };

        var jsonContent = JsonConvert.SerializeObject(emailContent);

        var request = new HttpRequestMessage(HttpMethod.Post, _emailConfig.ApiUrl)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _emailConfig.ApiKey);

        try
        {
            var response = await client.SendAsync(request);

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
