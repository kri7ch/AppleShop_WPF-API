using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ApplShopAPI.Services
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string From { get; set; } = "no-reply@appleshop.local";
    }

    public class EmailService
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IOptions<SmtpOptions> options, ILogger<EmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendReceiptAsync(string toEmail, byte[] pdfBytes, uint orderId)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) return;
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = $"AppleShop — Receipt for order #{orderId}";
            message.ReplyTo.Add(MailboxAddress.Parse(_options.From));
            message.Headers.Add(HeaderId.ListUnsubscribe, $"<mailto:{_options.From}?subject=unsubscribe>");

            var builder = new BodyBuilder
            {
                TextBody = $"Thank you for your purchase at AppleShop. Order #{orderId}. Your receipt (PDF) is attached. If you didn’t make this purchase, reply to this email.",
                HtmlBody = $@"<div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#111'>
                    <p>Hi,</p>
                    <p>Thank you for your purchase at <strong>AppleShop</strong>.</p>
                    <p>Order #: <strong>{orderId}</strong></p>
                    <p>Your PDF receipt is attached to this email.</p>
                    <p>If you did not make this purchase, please reply to this email.</p>
                    <p>Best regards,<br/>AppleShop Team</p>
                </div>"
            };
            builder.Attachments.Add($"Order_{orderId}.pdf", pdfBytes, new ContentType("application", "pdf"));
            message.Body = builder.ToMessageBody();

            var sslOption = _options.UseSsl? (_options.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls) : SecureSocketOptions.None;

            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.Timeout = 15000;
                await client.ConnectAsync(_options.Host, _options.Port, sslOption);
                await client.AuthenticateAsync(_options.User, _options.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation("Receipt email sent: to={To}, orderId={OrderId}", toEmail, orderId);
            }
            catch (MailKit.ServiceNotConnectedException ex)
            {
                _logger.LogError(ex, "SMTP not connected: to={To}, host={Host}, port={Port}", toEmail, _options.Host, _options.Port);
                throw;
            }
            catch (MailKit.ServiceNotAuthenticatedException ex)
            {
                _logger.LogError(ex, "SMTP authentication failed: to={To}, user={User}", toEmail, _options.User);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email: to={To}, host={Host}, port={Port}", toEmail, _options.Host, _options.Port);
                throw;
            }
        }
    }
}