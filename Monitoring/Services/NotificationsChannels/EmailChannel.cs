using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Monitoring.Models;
using Monitoring.Models.NotificationsModule.NotificationsChannels;

public class EmailChannel : INotificationChannel
{
    public async Task<bool> SendNotification(string content, Client recipient)
    {
        try
        {
            // Read SMTP configuration from environment variables
            string smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ;
            int smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")) ;
            string senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL");
            string senderPassword = Environment.GetEnvironmentVariable("SENDER_PASSWORD");

            var client = new SmtpClient(smtpServer, smtpPort)
            {

                Credentials = new NetworkCredential(senderEmail, senderPassword),

                EnableSsl = true

            };

            client.Send(senderEmail,recipient.Email , "Uptime Monitoring API Notif", content);

                return true;
            
        }
        catch (Exception ex)
        {
            // Log the error (optional)
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}