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

        StringBuilder content = new StringBuilder();
        content.AppendLine("Input: ");
        content.AppendLine(input.ToString());
        content.AppendLine("Output: ");
        content.AppendLine(output.ToString());

        emailContent.Content = content.ToString();

        return emailContent;
    }
}
