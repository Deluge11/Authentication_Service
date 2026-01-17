using Authentication_Core.Interfaces;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using ConstantsLib.Interfaces;


namespace Authentication_Infrastructure.Messaging
{
    public class RabbitMQPublisher : IEventBus
    {
        private readonly RabbitMQConnection _connection;

        public RabbitMQPublisher(RabbitMQConnection connection)
        {
            _connection = connection;
        }

        public async Task Publish<T>(T message) where T : IBaseEvent
        {
            if (!_connection.IsConnected) await _connection.TryConnect();

            using var channel = await _connection.CreateChannel();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
               exchange: message.Exchange.Name,
               routingKey: message.RoutingKey,
               mandatory: true,
               properties,
               body: body,
               cancellationToken: default
               );
        }
    }
}
