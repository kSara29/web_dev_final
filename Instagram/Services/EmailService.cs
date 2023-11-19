using MailKit.Net.Smtp;
using MimeKit;

namespace Instagram.Services;

public class EmailService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Администрация сайта", "instagramApp@gmail.com"));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };
        
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync("saratest727@gmail.com", "ljzd czqo upos vvho");
            await client.SendAsync(emailMessage);
            
            await client.DisconnectAsync(true);
        }
    }
    
    public async Task SendWelcomeAsync(string email, string userName, string link)
    {
        string subject = "Добро пожаловать!";
        string message = $"Добро пожаловать, {userName}!<br>Спасибо за регистрацию. Вы можете посмотреть ваш профиль по ссылке: {link}";

        await SendEmailAsync(email, subject, message);
    }
}