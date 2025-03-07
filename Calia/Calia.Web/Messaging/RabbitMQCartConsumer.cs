using Calia.Web.Models;
using Calia.Web.Service.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Calia.Web.Messaging
{
    public class RabbitMQCartConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory; // IServiceScopeFactory eklendi
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQCartConsumer(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory; // IServiceScopeFactory atanıyor

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                _configuration.GetValue<string>("TopicAndQueueNames:AddShoppingCartQueue"),
                false,
                false,
                false,
                null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                CartVM cartVM = JsonConvert.DeserializeObject<CartVM>(content);

                try
                {
                    await HandleMessage(cartVM, stoppingToken);  // Sepet güncelleme işlemi
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Hata yönetimi
                    Console.WriteLine($"Mesaj işleme hatası: {ex.Message}");
                }
            };

            _channel.BasicConsume(
                _configuration.GetValue<string>("TopicAndQueueNames:AddShoppingCartQueue"),
                false,
                consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CartVM cartVM, CancellationToken stoppingToken)
        {
            // Scoped bir servis oluşturuyoruz
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

                // Controller'daki UpsertCartAsync metodunu çağırıyoruz
                var response = await shoppingCartService.UpsertCartAsync(cartVM);

                if (response != null && response.IsSuccess)
                {
                    Console.WriteLine("Sepet başarıyla güncellendi.");
                }
                else
                {
                    Console.WriteLine($"Sepet güncelleme hatası: {response?.Message ?? "Bilinmeyen hata."}");
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}
