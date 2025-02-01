using RabbitMQ.Client;

namespace EDAS.WebApp.Services;

public class RabbitMQClientService : IDisposable, IAsyncDisposable
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly string _uri;
    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

    public RabbitMQClientService(string uri)
    {
        _uri = uri;
    }

    public async Task<IChannel> GetChannelAsync()
    {
        if (_connection == null || _channel == null || !_connection.IsOpen || _channel.IsClosed)
        {
            await _connectionLock.WaitAsync();
            try
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    await GetConnectionAsync();
                }

                if (_channel == null || _channel.IsClosed)
                {
                    _channel = await _connection.CreateChannelAsync();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        return _channel;
    }

    private async Task GetConnectionAsync()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(_uri)
        };

        var maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync();
                return;
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1)
                {
                    throw new Exception($"Failed to connect to RabbitMQ after {maxRetries} attempts.", ex);
                }

                await Task.Delay(delay);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
        }

        _channel = null;
        _connection = null;
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}