namespace EDAS.Worker;

public static class Utils
{
    public static EmailContent BuildEmailContent(string toEmail, 
        string subject, 
        CombinationsInputModel input, 
        CombinationsOutput output)
    {
        var emailContent = new EmailContent();
        emailContent.Title = subject;
        emailContent.ToEmail = toEmail;

        StringBuilder content = new StringBuilder();
        content.AppendLine("Input: ");
        content.AppendLine(input.ToString());
        content.AppendLine("Output: ");
        content.AppendLine(output.ToString());

        emailContent.Content = content.ToString();

        return emailContent;
    }
}
