using EDAS.Common.Services.RabbitMQ;
using EDAS.WebApp.Models;
using EDAS.WebApp.Services.Producers;

namespace EDAS.WebApp.Services.Factory;

public class ProducerFactory : IProducerFactory
{
    private readonly IRabbitMQClientService _rabbitMQClientService;
    private readonly BrokerConfig _brokerConfig;
    private readonly QueueConfigCollection _queueConfigCollection;

    public ProducerFactory(IRabbitMQClientService rabbitMQClientService,
        IOptions<BrokerConfig> brokerConfigOptions,
        QueueConfigCollection queueConfigCollection)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _brokerConfig = brokerConfigOptions.Value;
        _queueConfigCollection = queueConfigCollection;
    }

    public async Task<IProducerService> Create(ProducerFactoryConfig config)
    {
        var channel = await _rabbitMQClientService.GetChannelAsync();

        var exchangeConfig = new BrokerExchangeConfig
        {
            ExchangeName = _brokerConfig.ExchangeName,
            ExchangeType = _brokerConfig.ExchangeType,
            ExchangeKey = _queueConfigCollection.QueuesConfig[config.producerType].RoutingKey
        };

        return config.producerType switch
        {
            ProducerType.Combinatronics => new ProducerService(channel, exchangeConfig),
            ProducerType.Sorting => new ProducerService(channel, exchangeConfig),
            _ => throw new ArgumentException("ProducerFactory : Invalid option")
        };

    }
}
