namespace Calia.Web.Service.IService
{
    public interface IEmailSender
    {
        Task SendEmailWithAttachmentAsync(string email, string subject, string message, string filePath);

    }
}
