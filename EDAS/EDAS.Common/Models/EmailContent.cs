using System.Text.Json.Serialization;

namespace EDAS.Common.Models;

public class EmailContent
{
    [JsonPropertyName("from")]
    public EmailAddress From { get; set; }

    [JsonPropertyName("to")]
    public List<EmailAddress> To { get; set; }

    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonPropertyName("html")]
    public string Html { get; set; }
}

public class EmailAddress
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
}
