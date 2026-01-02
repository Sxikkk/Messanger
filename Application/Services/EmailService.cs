using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailService(IConfiguration config)
    {
        _smtpHost = config["Email:SmtpHost"] ?? string.Empty;
        _smtpPort = int.Parse(config["Email:SmtpPort"] ?? string.Empty);
        _fromEmail = config["Email:FromEmail"] ?? string.Empty;
        _smtpUser = config["Email:SmtpUser"] ?? string.Empty;
        _smtpPass = config["Email:SmtpPass"] ?? string.Empty;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_fromEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_smtpUser, _smtpPass, cancellationToken);
        await client.SendAsync(email, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}