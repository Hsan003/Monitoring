using System.Net.Mail;
using Monitoring.Models;
using Monitoring.Models.NotificationsModule.NotificationsChannels;

public class EmailChannel : INotificationChannel
{
    private readonly string smtpServer;
    private readonly int smtpPort;
    private readonly string senderEmail;
    private readonly string senderPassword;

    public EmailChannel(string smtpServer, int smtpPort, string senderEmail, string senderPassword)
    {
        this.smtpServer = smtpServer;
        this.smtpPort = smtpPort;
        this.senderEmail = senderEmail;
        this.senderPassword = senderPassword;
    }

    public async Task<bool> SendNotification(string content, Client recipient)
    {
        
        try
        {
            string smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? "localhost";
            int smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "25");
            string senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL") ?? "no-reply@example.com";
            string senderPassword = Environment.GetEnvironmentVariable("SENDER_PASSWORD") ?? "";

            using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = !string.IsNullOrWhiteSpace(senderPassword)
                    ? new System.Net.NetworkCredential(senderEmail, senderPassword)
                    : null;
                smtpClient.EnableSsl = smtpPort == 587 || smtpPort == 465; // SSL only if using standard SMTP secure ports

                var mailMessage = new MailMessage(senderEmail, recipient.Email)
                {
                    Subject = "Notification",
                    Body = content
                };

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
        }
        catch (Exception ex)
        {
            // Log the error (optional)
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}
