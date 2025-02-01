namespace EDAS.AzureFunction.Email.Model;

public class EmailRequest
{
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}
