using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ServiceBook.EmailService;

public class EmailService : IEmailService
{
	private string Email => "rav3develop@gmail.com";
	private string Password => "2012Artem.2012";
	private string Host => "smtp-mail.outlook.com";
	private int Port => 587;

	public async Task SendEmailAsync(string email, string subject, string message)
	{
		var emailMessage = new MimeMessage();
		emailMessage.From.Add(new MailboxAddress("Администрация сайта", Email));
		emailMessage.To.Add(new MailboxAddress("", email));

		emailMessage.Subject = subject;
		emailMessage.Body = new BodyBuilder()
		{
			HtmlBody = message
		}.ToMessageBody();

		using (var client = new SmtpClient())
		{
			await client.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
			await client.AuthenticateAsync(Email, Password);
			await client.SendAsync(emailMessage);

			await client.DisconnectAsync(true);
		}
	}
}