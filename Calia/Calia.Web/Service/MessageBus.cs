using Azure.Messaging.ServiceBus;
using Calia.Web.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace Calia.Web.Service
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://callia.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bxaiHO6fW88hIYg+1X7QXLAuIoeDmFqY8+ASbJdRhLg=";
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),


            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
