using EDAS.Common;
using EDAS.WebApp.Models;
using RabbitMQ.Client;
using System.Text;

namespace EDAS.WebApp.Services;

public class ProducerService : IProducerService
{

    private readonly RabbitMQClientService _rabbitMQClientService;
    

    public ProducerService(RabbitMQClientService rabbitMQClientService)
    {
        _rabbitMQClientService = rabbitMQClientService;
    }

    public async Task SendMessageAsync(ProducerMessage message)
    {
        using var channel = await _rabbitMQClientService.GetChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: RabbitMQConfig.EXCHANGE_NAME,
            type: RabbitMQConfig.EXCHANGE_TYPE);

        var body = Encoding.UTF8.GetBytes(message.Message);

        await channel.BasicPublishAsync(exchange: RabbitMQConfig.EXCHANGE_NAME, 
            routingKey: RabbitMQConfig.ROUTING_KEY, 
            body: body);
    }
}
