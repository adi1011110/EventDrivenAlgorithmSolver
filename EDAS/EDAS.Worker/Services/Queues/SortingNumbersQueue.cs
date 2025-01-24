namespace EDAS.Worker.Services.Queues;

public class SortingNumbersQueue : BaseQueue, IRabbitMQueue
{
    public SortingNumbersQueue(IChannel channel, 
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

        var input = JsonConvert.DeserializeObject<SortingInputModel>(message);

        try
        {
            var command = _mapper.Map<SortingInputCommand>(input);

            var output = await mediator.Send(command);

            var emailContent = EmailContentBuilder.BuildEmailContent(
                    input.EmailAddress,
                    "Sorting solution",
                    input,
                    output);

            await emailService.SendEmailAsync(emailContent.ToEmail,
                emailContent.Title,
                emailContent.Content);

            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch(Exception e)
        {
            return;
        }
    }
}
