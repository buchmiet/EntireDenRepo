using Answers;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace EmailService;

public interface IEmailService
{
    Task<Answer> SendEmail(string from, string username, string password, string to, string subject, string content);
}

public class EmailSettings
{
    public string Username { get; set; } 
    public string Password { get; set; } 
    public string Smtp { get; set; }
}

public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task<Answer> SendEmail(string from, string username, string password, string to, string subject, string content)
    {
        var answer = Answer.Prepare($"Sending email(u/p:'{username}'/'{password}') from {from} to {to} with subject '{subject}' and content '{content}'");
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from)); 
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Plain) { Text = content };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.ServerCertificateValidationCallback = (_, _, _, _) => true;

        try
        {
            await client.ConnectAsync(_emailSettings.Smtp, 587, SecureSocketOptions.StartTls).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return answer.Error($"when connecting: {ex.GetType()}:{ex.Message}");
        }

        try
        {
            await client.AuthenticateAsync(username, password).ConfigureAwait(false); 
        }
        catch (Exception ex)
        {
            return answer.Error($"when authenticating: {ex.GetType()}:{ex.Message}");
        }
        try
        {
            await client.SendAsync(email).ConfigureAwait(false); 
        }
        catch (Exception ex)
        {
            return answer.Error($"when sending: {ex.GetType()}:{ex.Message}");
        }
        try
        {
            await client.DisconnectAsync(true).ConfigureAwait(false); ;
        }
        catch (Exception ex)
        {
            return answer.Error($"when sending: {ex.GetType()}:{ex.Message}");
        }
        return answer;
    }

}