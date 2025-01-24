namespace EDAS.Worker.Utils;

public static class EmailContentBuilder
{
    public static EmailContent BuildEmailContent<TInput, TOutput>(string toEmail,
        string subject,
        TInput input,
        TOutput output)
    {
        if((!VerifyToString.IsOverriden<TInput>()) || (!VerifyToString.IsOverriden<TOutput>()))
        {
            throw new Exception("Generic arguments need to overried ToString() method");
        }

        var emailContent = new EmailContent();
        emailContent.Title = subject;
        emailContent.ToEmail = toEmail;
        string newHtmlLine = "<br>";

        StringBuilder content = new StringBuilder();
        content.AppendLine("Input:");
        content.AppendLine(newHtmlLine);

        content.AppendLine(input.ToString());
        content.AppendLine(newHtmlLine);

        content.AppendLine("Output:");
        content.AppendLine(newHtmlLine);

        content.AppendLine(output.ToString());
        content.AppendLine(newHtmlLine);

        emailContent.Content = content.ToString();

        return emailContent;
    }
}
