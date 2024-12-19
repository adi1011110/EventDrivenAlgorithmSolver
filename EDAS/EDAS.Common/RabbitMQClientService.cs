using RabbitMQ.Client;

namespace EDAS.WebApp.Services;

public class RabbitMQClientService : IDisposable
{

    private IConnection _connection;
    private IChannel _channel;
    private readonly string _hostname;
    private readonly string _username;
    private readonly string _password;

    public RabbitMQClientService(string hostname, string username, string password)
    {
        _hostname = hostname;
        _username = username;
        _password = password;
    }

    public async Task<IChannel> GetChannelAsync()
    {
        if(_connection == null || (!_connection.IsOpen))
        {
            await GetConnectionAsync();
        }

        if (_channel == null || _channel.IsClosed)
        {            
            _channel = await _connection.CreateChannelAsync();
        }

        return _channel;
    }

    private async Task GetConnectionAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            UserName = _username,
            Password = _password,
            Port = 5672
        };

        var maxRetries = 10; // Number of retries
        var delay = TimeSpan.FromSeconds(5); // Time between retries

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                await Task.Delay(delay);
            }
        }
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
