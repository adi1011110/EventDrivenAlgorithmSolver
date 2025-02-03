using EDAS.Common.Services.Email;

namespace EDAS.Worker.Services.Queues;

public class GenericHandler<TInputModel, TInputCommand, TOutputCommand> where TInputModel : BaseInputModel
{
    private readonly IServiceProvider _serviceProvider;

    public GenericHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(object model, BasicDeliverEventArgs ea)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var input = JsonConvert.DeserializeObject<TInputModel>(message);

            var algorithmCommand = mapper.Map<TInputCommand>(input);
            TOutputCommand output = (TOutputCommand)await mediator.Send(algorithmCommand);

            var emailRequest = EmailRequestBuilder.BuildEmailRequest(
                input.EmailAddress,
                input.Subject,
                input,
                output);

            await emailService.SendEmailAsync(emailRequest);
        }
        catch(Exception e)
        {
            var exText = e.Message.ToString();
            int a = 5;
        }
    }
}