using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Monitoring.Models;

namespace Monitoring.Services;

public class EmailSender : IEmailSender<Client>
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public async Task SendConfirmationLinkAsync(Client user, string email, string confirmationLink)
    {
        string subject = "Confirm your email";
        string body = $"<p>Please confirm your email by clicking <a href='{confirmationLink}'>here</a></p>";

        await SendEmailAsync(email, subject, body);
    }

    public async  Task SendPasswordResetLinkAsync(Client user, string email, string resetLink)
    {
        string subject = "Reset Your Password";
        string body = $"<p>To reset your password, please click <a href='{resetLink}'>here</a>.</p><p>If you did not request this, please ignore this email.</p>";

        await SendEmailAsync(email, subject, body);
    }

    public Task SendPasswordResetCodeAsync(Client user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        string smtpServer = "smtp.gmail.com"; 
        int smtpPort = 587;
        string smtpUser = "cp.online.coach@gmail.com"; 
        string smtpPassword = "ythx wdti xymb anfp"; 

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpUser),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
            smtpClient.EnableSsl = true;

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending email: {Message}", ex.Message);
            }
        }
    }
}