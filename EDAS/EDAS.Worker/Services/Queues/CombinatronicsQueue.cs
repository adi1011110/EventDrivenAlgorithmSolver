namespace EDAS.Worker.Services.Queues;

public class CombinatronicsQueue : 
    BaseQueue<CombinationsInputModel, CombinationsInputCommand, CombinationsOutput>, 
    IRabbitMQueue
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
        try 
        {
            await base.HandleMessage(model, ea);
        }
        catch (Exception e)
        {
            return;
        }
    }
}
