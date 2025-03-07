using Azure;
using EDAS.Common.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace EDAS.Common.Services.Email;

public class AzureEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly AzureFunctionConfig _emailData;
    private readonly string _functionUrl;

    public AzureEmailService(IHttpClientFactory httpClientFactory, IOptions<AzureFunctionConfig> emailDataOptions)
    {
        _httpClient = httpClientFactory.CreateClient();
        _emailData = emailDataOptions.Value;
        _functionUrl = $"{_emailData.AppFunctionUrl}?code={_emailData.AppFunctionKey}";
    }

    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        if (emailRequest == null ||
            string.IsNullOrEmpty(emailRequest.Email) ||
            string.IsNullOrEmpty(emailRequest.Message) ||
            string.IsNullOrEmpty(emailRequest.Subject))
        {
            throw new ArgumentException("Email requests fields are null");
        }

        var requestBody = new
        {
            Email = emailRequest.Email,
            Subject = emailRequest.Subject,
            Message = emailRequest.Message
        };

        HttpResponseMessage response = null;

        try
        {
            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            response = await _httpClient.PostAsync(_functionUrl, content);
            return response.IsSuccessStatusCode;
        }
        catch(Exception e)
        {
            return false;
        }
    }
}
