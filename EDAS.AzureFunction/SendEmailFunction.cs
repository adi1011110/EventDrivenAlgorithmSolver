namespace EDAS.AzureFunction.Email.Function;

public class SendEmailFunction
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendEmailFunction> _logger;

    public SendEmailFunction(IEmailService emailService,
            ILogger<SendEmailFunction> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [Function("SendEmail")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")]
                                            HttpRequestData req)
    {
        _logger.LogInformation("Received email request.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var emailRequest = JsonSerializer.Deserialize<EmailRequest>(requestBody);

        if (emailRequest == null || string.IsNullOrEmpty(emailRequest.Email))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request.");
            return badResponse;
        }

        await _emailService.SendEmailAsync(emailRequest);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Email sent successfully.");
        return response;
    }
}
