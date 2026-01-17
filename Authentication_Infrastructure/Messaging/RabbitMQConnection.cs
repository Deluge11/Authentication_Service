using ConstantsLib.Exchanges;
using ConstantsLib.Interfaces;
using RabbitMQ.Client;

public class RabbitMQConnection : IDisposable, IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private bool _disposed;
    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

    public RabbitMQConnection(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    public async Task<bool> TryConnect(CancellationToken cancellationToken = default)
    {
        if (IsConnected) return true;

        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (IsConnected) return true;

            _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken: cancellationToken);

            return IsConnected;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<IChannel> CreateChannel(CancellationToken cancellationToken = default)
    {
        if (!IsConnected && !await TryConnect(cancellationToken))
        {
            throw new InvalidOperationException("Cannot create RabbitMQ channel: connection failed.");
        }

        return await _connection!.CreateChannelAsync(null, cancellationToken);
    }

    public async Task InitializeInfrastructure(CancellationToken cancellationToken = default)
    {
        await using var channel = await CreateChannel(cancellationToken);

        IExchange exchange = new AuthExchange();

        await channel.ExchangeDeclareAsync(
            exchange.Name,
            exchange.Type,
            exchange.IsDurable, 
            cancellationToken: cancellationToken);

    }

    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        if (_connection != null)
        {
            try
            {
                await _connection.DisposeAsync();
            }
            catch
            {
                // Log if needed
            }
        }

        _connectionLock?.Dispose();
    }
}
