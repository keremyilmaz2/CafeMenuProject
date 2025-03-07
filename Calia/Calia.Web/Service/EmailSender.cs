using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Calia.Web.Service
{
    public class EmailSender : Calia.Web.Service.IService.IEmailSender
    {
        

        public async Task SendEmailWithAttachmentAsync(string email, string subject, string message, string filePath)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Callia", "keremyilmazeng@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message // HTML içeriği
            };

            // Dosya ekle
            if (File.Exists(filePath))
            {
                bodyBuilder.Attachments.Add(filePath);
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                string appPassword = "axbl tjfz omzf afks";
                // SMTP ayarlarını buraya ekleyin
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("keremyilmazeng@gmail.com", appPassword);

                await client.SendAsync(emailMessage);
                client.Disconnect(true);
            }
        }
    }
}
