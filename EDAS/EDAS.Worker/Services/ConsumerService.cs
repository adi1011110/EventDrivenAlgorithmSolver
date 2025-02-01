namespace EDAS.Worker.Services;

public class ConsumerService : BackgroundService
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private IChannel _channel;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqConfig _rabbitMqConfig;

    public ConsumerService(RabbitMQClientService rabbitMQClientService,
        IMapper mapper,
        IServiceProvider serviceProvider,
        RabbitMqConfig rabbitMqConfig)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
        _rabbitMqConfig = rabbitMqConfig;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _rabbitMQClientService.GetChannelAsync();

        using var scope = _serviceProvider.CreateScope();

        var queueFactory = scope.ServiceProvider.GetRequiredService<IQueueFactory>();

        //solve
        var queueType = QueueHelper.ConvertStringToEnum(_rabbitMqConfig.AlgorithmType);

        var queueFacoryConfig = new QueueFactoryConfig(_channel, 
            _rabbitMqConfig, 
            _serviceProvider, 
            _mapper, 
            queueType);

        var queue = queueFactory.Create(queueFacoryConfig);

        await queue.StartConsuming();

        while (!stoppingToken.IsCancellationRequested) { }
    }
}
