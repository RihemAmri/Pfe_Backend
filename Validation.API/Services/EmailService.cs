using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Validation.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendValidationEmailAsync(string toEmail, byte[] pdfContent)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"]),
                Subject = "Félicitations - Accord de principe",
                Body = "Félicitations, votre demande a été validée. Vous trouverez ci-joint la notification d'accord de principe.",
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(pdfContent), "Notification_Accord.pdf", "application/pdf"));

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
