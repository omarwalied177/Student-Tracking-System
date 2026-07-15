using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StudentTrackingSystem.Pl.Services;


public class EmailService : IEmailService
{
    private readonly MailSettings _mailSettings;

    public EmailService(IConfiguration configuration)
    {
        _mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>();
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpClient = new SmtpClient(_mailSettings.Host)
        {
            Port = _mailSettings.Port,
            Credentials = new NetworkCredential(_mailSettings.Email, _mailSettings.Password),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_mailSettings.Email, _mailSettings.DisplayName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(toEmail);

        try
        {
            // إرسال البريد
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // لو حصل خطأ، اعرضه
            throw new InvalidOperationException($"Email sending failed: {ex.Message}");
        }
    }
}
