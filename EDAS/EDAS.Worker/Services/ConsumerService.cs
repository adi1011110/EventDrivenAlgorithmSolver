using EDAS.Common;
using EDAS.WebApp.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EDAS.Worker.Services;

public class ConsumerService : BackgroundService
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private IChannel _channel;

    public ConsumerService(RabbitMQClientService rabbitMQClientService)
    {
        _rabbitMQClientService = rabbitMQClientService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _channel = await _rabbitMQClientService.GetChannelAsync();

        await _channel.ExchangeDeclareAsync(exchange: RabbitMQConfig.EXCHANGE_NAME,
                                    type: RabbitMQConfig.EXCHANGE_TYPE);

        await _channel.QueueDeclareAsync(
            queue: RabbitMQConfig.QUEUE_NAME,
            durable: RabbitMQConfig.QUEUE_DURABLE,
            exclusive: RabbitMQConfig.QUEUE_EXCLUSIVE,
            autoDelete: RabbitMQConfig.QUEUE_AUTODELETE,
            arguments: null);

        await _channel.QueueBindAsync(queue: RabbitMQConfig.QUEUE_NAME, 
            exchange: RabbitMQConfig.EXCHANGE_NAME,
            routingKey: RabbitMQConfig.ROUTING_KEY);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            //received message, pass it to the algo solver

            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        await _channel.BasicConsumeAsync(queue: RabbitMQConfig.QUEUE_NAME, 
            autoAck: false, 
            consumer: consumer);
    }
}
