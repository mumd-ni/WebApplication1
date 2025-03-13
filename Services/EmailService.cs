 // Services/EmailService.cs
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

namespace WebApplication1.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        // حقن IConfiguration لقراءة الإعدادات من appsettings.json
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // إنشاء رسالة بريد إلكتروني
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"]
            ));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            // إعداد نص الرسالة (نص عادي أو HTML)
            email.Body = new TextPart("plain") // استخدم "html" لتنسيق HTML
            {
                Text = body
            };

            // إعداد عميل SMTP
            using (var smtp = new SmtpClient())
            {
                try
                {
                    // الاتصال بخادم SMTP
                    await smtp.ConnectAsync(
                        _configuration["EmailSettings:SmtpServer"],
                        int.Parse(_configuration["EmailSettings:SmtpPort"]),
                        SecureSocketOptions.StartTls // استخدام TLS للأمان
                    );

                    // مصادقة المستخدم
                    await smtp.AuthenticateAsync(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]
                    );

                    // إرسال الرسالة
                    await smtp.SendAsync(email);

                    // قطع الاتصال
                    await smtp.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to send email: {ex.Message}", ex);
                }
            }
        }
    }
}
