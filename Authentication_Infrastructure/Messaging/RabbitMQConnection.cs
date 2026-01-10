using Authentication_Core.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;




namespace Authentication_Infrastructure.Messaging
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private bool _disposed;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public async Task<bool> TryConnect()
        {
            if (IsConnected) return true;

            await _connectionLock.WaitAsync();

            try
            {
                if (IsConnected) return true;

                _connection = await _connectionFactory.CreateConnectionAsync();

                return IsConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot connect with RabbitMQ : {ex.Message}");
                return false;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<IChannel> CreateChannel()
        {
            if (!IsConnected && !await TryConnect())
            {
                throw new InvalidOperationException("Cannot Create RabbitMQ Channel");
            }

            return await _connection!.CreateChannelAsync();
        }
        public async Task InitializeInfrastructure()
        {
            using var channel = await CreateChannel();

            await channel.ExchangeDeclareAsync("auth.events", ExchangeType.Topic, durable: true);
        }

        public void Dispose()
        {
            _disposed = true;
            _connection?.Dispose();
        }
    }
}
