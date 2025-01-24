namespace EDAS.Worker.Services.Queues;

public abstract class BaseQueue : IRabbitMQueue
{
    protected readonly IChannel _channel;
    protected readonly RabbitMqConfig _rabbitMqConfig;
    protected readonly IServiceProvider _serviceProvider;
    protected readonly IMapper _mapper;

    public BaseQueue(IChannel channel,
        RabbitMqConfig rabbitMqConfigOption,
        IServiceProvider serviceProvider,
        IMapper mapper)
    {
        _channel = channel;
        _rabbitMqConfig = rabbitMqConfigOption;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }

    public virtual async Task StartConsuming()
    {
        //exchange/queue setup

        await _channel.ExchangeDeclareAsync(exchange: _rabbitMqConfig.ExchangeName,
                type: _rabbitMqConfig.ExchangeType);

        bool queueDurable = bool.TryParse(_rabbitMqConfig.QueueDurable, out bool queueDurableResult);
        bool queueExclusive = bool.TryParse(_rabbitMqConfig.QueueExclusive, out bool queueExclusiveResult);
        bool autoDelete = bool.TryParse(_rabbitMqConfig.QueueAutodelete, out bool autoDeleteResult);

        await _channel.QueueDeclareAsync(
            queue: _rabbitMqConfig.QueueName,
            durable: queueDurable,
            exclusive: queueExclusive,
            autoDelete: autoDelete,
            arguments: null);

        await _channel.QueueBindAsync(queue: _rabbitMqConfig.QueueName,
            exchange: _rabbitMqConfig.ExchangeName,
            routingKey: _rabbitMqConfig.RoutingKey);

    }

    protected abstract Task HandleMessage(object model, BasicDeliverEventArgs ea);
}
