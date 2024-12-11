using RabbitMQ.Client;

namespace EDAS.WebApp.Services;

public class RabbitMQClientService : IDisposable
{

    private readonly IConnection _connection;
    private IChannel _channel;

    public RabbitMQClientService(string hostname, string username, string password)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostname,
            UserName = username,
            Password = password
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public async Task<IChannel> GetChannelAsync()
    {
        if (_channel == null || _channel.IsClosed)
        {
            _channel = await _connection.CreateChannelAsync();
        }

        return _channel;
    }

    public void Dispose()
    {
        if (_channel != null && _channel.IsOpen)
        {
            _channel.CloseAsync().GetAwaiter().GetResult();
        }

        if (_connection != null && _connection.IsOpen)
        {
            _connection.CloseAsync().GetAwaiter().GetResult();
        }
    }
}
