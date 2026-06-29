using System.Text;
using System.Text.Json;
using Application.Interfaces;
using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class RabbitMqPublisher(RabbitMqOptions options) : IMessagePublisher, IAsyncDisposable
    {
        private IConnection? _connection;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        private async Task<IConnection> GetConnectionAsync(CancellationToken ct)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            await _connectionLock.WaitAsync(ct);
            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                var factory = new ConnectionFactory
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    UserName = options.UserName,
                    Password = options.Password
                };

                _connection = await factory.CreateConnectionAsync(ct);
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task PublishAsync<T>(string queueName, T message, CancellationToken ct = default)
        {
            var connection = await GetConnectionAsync(ct);
            using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

            // Idempotent: đảm bảo queue tồn tại trước khi publish (demo nên dùng exchange mặc định "")
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body,
                cancellationToken: ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
                await _connection.DisposeAsync();

            _connectionLock.Dispose();
        }
    }
}