using MailKit.Net.Smtp;
using MailKit.Security;
using UpLoader_For_ET.Configuration;
using UpLoader_For_ET.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace UpLoader_For_ET.Services
{
    public class MailService : IEmailSender
    {

        private readonly MailSettings _settings;

        /// <summary>  
        /// grabs relevant data from json in appsettings.json via middleware registering builder.Services.Configure<MailSettings>
        /// </summary>
        /// <param name="settings"></param>
        public MailService(IOptions<MailSettings> settings) 
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            List<string> recipients = new() { email };
            MailData mailData = new(to: recipients, subject: subject, body: htmlMessage);

            bool sendAttemptSuccessful = await SendAsync(mailData);

            if(sendAttemptSuccessful)
            {
                Console.WriteLine("Mail == Sent, relief washes over me in an awesome wave.");
                
            }
            else
            {
                Console.WriteLine($"mail error: ");
            }

            await Task.CompletedTask;
        }

        public async Task<bool> SendAsync(MailData mailData, CancellationToken ct = default)
        {
            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
                mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

                // Receiver
                foreach (string mailAddress in mailData.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if(!string.IsNullOrEmpty(mailData.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (mailData.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = mailData.Subject;
                body.HtmlBody = mailData.Body;
                mail.Body = body.ToMessageBody();

                #endregion

                #region Send Mail

                using var smtp = new SmtpClient();

                if (_settings.UseSSL)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (_settings.UseStartTls)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
                }
                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);
                
                #endregion

                return true;

            }
            catch (Exception except)
            {
                Console.WriteLine(except.Message);
                return false;
            }
        }
    }
}