namespace Calia.Web.Service
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();

    }
}
