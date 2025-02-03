namespace EDAS.Worker.Services.Queues;

public class SortingNumbersQueue : 
    BaseQueue<SortingInputModel, SortingInputCommand, SortingOutputResult>, 
    IRabbitMQueue
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
        try 
        {
            await _genericHandler.Handle(model, ea);

        }
        catch(Exception e)
        {
            return;
        }
    }
}
