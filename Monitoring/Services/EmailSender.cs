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
        throw new NotImplementedException();
    }

    public async Task SendPasswordResetCodeAsync(Client user, string email, string resetToken)
    {
        string subject = "Password Reset Request";
        string body = $@"
        <p>We received a request to reset your password.</p>
        <p>Your User ID: <strong>{user.Id}</strong></p>
        <p>Your Reset Token: <strong>{resetToken}</strong></p>
        <p>Use these credentials in the Reset Password API.</p>";

        await SendEmailAsync(email, subject, body);
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