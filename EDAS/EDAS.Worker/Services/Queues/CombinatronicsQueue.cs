using EDAS.Worker.Handlers.Commands.Combinations;

namespace EDAS.Worker.Services.Queues;

public class CombinatronicsQueue : BaseQueue, IRabbitMQueue
{
    public CombinatronicsQueue(IChannel channel,
        RabbitMqConfig rabbitMqConfigOption,
        IServiceProvider serviceProvider,
        IMapper mapper) : base(channel, rabbitMqConfigOption, serviceProvider, mapper)
    {
    }

    public override async Task StartConsuming()
    {
        await base.StartConsuming();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += HandleMessage;

        await _channel.BasicConsumeAsync(queue: _rabbitMqConfig.QueueName,
            autoAck: false,
            consumer: consumer);
    }

    protected override async Task HandleMessage(object model, BasicDeliverEventArgs ea)
    {
        using var scope = _serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var emailService = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var inputModel = JsonConvert.DeserializeObject<CombinationsInputModel>(message);
        try
        {
            //1. Solve algorithm
            var algorithmCommand = _mapper.Map<CombinationsInput>(inputModel);
            var combinationsOutput = await mediator.Send(algorithmCommand);

            //2. Build/send email
            var emailContent = Utils.BuildEmailContent(
                inputModel.EmailAddress,
                "Combinations solution",
                inputModel,
                combinationsOutput);

            await emailService.SendEmailAsync(emailContent.ToEmail,
                emailContent.Title,
                emailContent.Content);

            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch (Exception e)
        {
            //message will not be validated hence it will be sent by the broker again after some time
            //log exception here
            return;
        }
    }
}
