using Microsoft.Extensions.Hosting;



namespace Authentication_Infrastructure.Messaging
{
    public class RabbitMQInitializer : IHostedService
    {
        private readonly RabbitMQConnection _connection;

        public RabbitMQInitializer(RabbitMQConnection connection)
        {
            _connection = connection;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _connection.InitializeInfrastructure();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
