using Application.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Infrastructure.Messaging.Consumers
{
    public class UserRegisteredConsumer(RabbitMqOptions options, ILogger<UserRegisteredConsumer> logger)
        : BackgroundService
    {
        private const string QueueName = "user.registered";
        private IConnection? _connection;
        private IChannel? _channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };

            // RabbitMQ container có thể chưa sẵn sàng khi service này start -> retry vài lần
            while (!stoppingToken.IsCancellationRequested && _connection is null)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("RabbitMQ chưa sẵn sàng ({Message}), thử lại sau 5s...", ex.Message);
                    await Task.Delay(5000, stoppingToken);
                }
            }

            if (_connection is null) return;

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var evt = JsonSerializer.Deserialize<UserRegisteredEvent>(json);

                    logger.LogInformation(
                        "[user.registered] Đã nhận event: gửi welcome email cho {Username} ({Email}), đăng ký lúc {CreatedAt}",
                        evt?.Username, evt?.Email, evt?.CreatedAt);

                    await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi xử lý message từ queue {Queue}", QueueName);
                    await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            // Giữ service sống tới khi app shutdown
            await Task.Delay(Timeout.Infinite, stoppingToken).ContinueWith(_ => { });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null) await _channel.DisposeAsync();
            if (_connection is not null) await _connection.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}