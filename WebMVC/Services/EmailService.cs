
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using WebMVC.OptionsModel;

namespace WebMVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _emailSettings = settings.Value;
        }

        public async Task SendResetPasswordEmail(string resetPasswordEmailLink, string ToEmail)
        {
            var smptClient = new SmtpClient();

            // Smtp Ayarları yapılıyor
            smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smptClient.UseDefaultCredentials = false;
            smptClient.Port = 587;
            smptClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
            smptClient.EnableSsl = true;
            smptClient.Host = _emailSettings.Host;

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_emailSettings.Email);
            mailMessage.To.Add(ToEmail);

            mailMessage.Subject = "Localhost | Şifre sıfırlama linki";
            mailMessage.Body = $@"
                <h4>Şifrenizi yenilemek için aşağıdaki linke tıklayınız.</h4>
                <p><a href='{resetPasswordEmailLink}'> şifre yenileme link</a></p>";

            mailMessage.IsBodyHtml = true; //Mail in body kısmında html kullanmaya izin ver komutu

            await smptClient.SendMailAsync(mailMessage);

        }
    }
}
