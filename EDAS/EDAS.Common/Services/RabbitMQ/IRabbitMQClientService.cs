using RabbitMQ.Client;

namespace EDAS.Common.Services.RabbitMQ;

public interface IRabbitMQClientService : IDisposable, IAsyncDisposable
{
    Task<IChannel> GetChannelAsync();
}
