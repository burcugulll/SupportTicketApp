using System.Net.Mail;
using System.Net;

namespace SupportTicketApp.Utils
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _port = 587;
        private readonly string _username = "burcumailtest@gmail.com";
        private readonly string _password = "fadj vhob fmxj gmtk";

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer))
                {
                    smtpClient.Port = _port;
                    smtpClient.Credentials = new NetworkCredential(_username, _password);
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_username),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(recipientEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                Console.WriteLine($"E-posta gönderimi sırasında bir hata oluştu: {ex.Message}");
            }
        }
    }
}
