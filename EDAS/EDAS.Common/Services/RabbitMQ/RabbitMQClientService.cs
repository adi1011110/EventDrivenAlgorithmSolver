using EDAS.Common.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EDAS.Common.Services.RabbitMQ;

public class RabbitMQClientService<TConfig> : IRabbitMQClientService
    where TConfig : class
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly TConfig _config;
    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

    public RabbitMQClientService(IOptions<TConfig> config)
    {
        _config = config.Value;
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
        var factory = GetConnectionFactory(_config);

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

    private ConnectionFactory GetConnectionFactory(TConfig config)
    {
        ConnectionFactory factory = null;

        switch (config)
        {
            case RabbitMQLocalConfig localConfig:
                factory = new ConnectionFactory
                {
                    HostName = localConfig.Hostname,
                    UserName = localConfig.Username,
                    Password = localConfig.Password,
                    Port = int.Parse(localConfig.Port)
                };
                break;

            case RabbitMQAzureConfig azureConfig:
                factory = new ConnectionFactory()
                {
                    Uri = new Uri(azureConfig.Url)
                };
                break;

            default:
                throw new ArgumentException($"Unsupported config type: {typeof(TConfig).Name}");
        }

        return factory;
    }
}