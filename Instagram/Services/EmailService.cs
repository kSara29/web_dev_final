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
            await client.AuthenticateAsync("instagramapp912@gmail.com", "pdjl xkxa tacz raho");
            await client.SendAsync(emailMessage);
            
            await client.DisconnectAsync(true);
        }
    }
    
    public async Task SendWelcomeEmailAsync(string email, string userName, string link)
    {
        string subject = "Добро пожаловать!";
        string message = $"Добро пожаловать, {userName}!<br>Спасибо за регистрацию. Вы можете посмотреть ваш профиль по ссылке: {link}";

        await SendEmailAsync(email, subject, message);
    }
    
    public async Task SendUserEditEmailAsync(string email, string changesInfo)
    {
        string subject = "Редактирование профиля";
        string message = $"Редактирование прошло усешно! Внесенные изменения:<br>{changesInfo}";

        await SendEmailAsync(email, subject, message);
    }
    
    public async Task SendUserDataEmailAsync(string email, string userData)
    {
        string subject = "Запрос данных профиля";
        string message = $"Ваш запрос данных профиля:<br>{userData}";

        await SendEmailAsync(email, subject, message);
    }
}