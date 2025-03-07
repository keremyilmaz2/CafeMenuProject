using Azure.Messaging.ServiceBus;
using Calia.Web.Models;
using Calia.Web.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace Calia.Web.Service
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ServiceBusProcessor _emailCartProcessor;
        
        public AzureServiceBusConsumer(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
           
            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            
        }

        public  async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

        }

        

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

        } 

        private  Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());   
            return Task.CompletedTask;
        }

        private  async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);
                CartVM objMessage = JsonConvert.DeserializeObject<CartVM>(body);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

                    // Controller'daki UpsertCartAsync metodunu çağırıyoruz
                    var response = await shoppingCartService.UpsertCartAsync(objMessage);

                    if (response != null && response.IsSuccess)
                    {
                        Console.WriteLine("Sepet başarıyla güncellendi.");
                    }
                    else
                    {
                        Console.WriteLine($"Sepet güncelleme hatası: {response?.Message ?? "Bilinmeyen hata."}");
                    }
                }


                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

       


    }
}
