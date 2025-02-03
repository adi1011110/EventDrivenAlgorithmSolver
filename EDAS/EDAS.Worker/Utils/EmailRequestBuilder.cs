namespace EDAS.Worker.Utils;

public static class EmailRequestBuilder
{
    public static EmailRequest BuildEmailRequest<TInput, TOutput>(string toEmail,
        string subject,
        TInput input,
        TOutput output)
    {
        if((!VerifyToString.IsOverriden<TInput>()) || (!VerifyToString.IsOverriden<TOutput>()))
        {
            throw new Exception("Generic arguments need to overried ToString() method");
        }

        var emailContent = new EmailRequest();
        emailContent.Subject = subject;
        emailContent.Email = toEmail;
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

        emailContent.Message = content.ToString();

        return emailContent;
    }
}
