using OBPostupyApi.Settings;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

namespace OBPostupyApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_emailSettings.EmailAddress, _emailSettings.EmailName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
        }
    }
}
