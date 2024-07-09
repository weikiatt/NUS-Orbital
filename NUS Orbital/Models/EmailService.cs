using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NUS_Orbital.Models
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("tft6261@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("tft6261@gmail.com", "jurl urzt pgcp nzpz");
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task SendVerificationCodeAsync(string toEmail, string verificationCode)
        {
            string subject = "NUS Orbital TFT Verification Code";
            string body = $"Your verification code is: {verificationCode}, it expires in 5 minutes." ;
            await SendEmailAsync(toEmail, subject, body);
        }
    }
}