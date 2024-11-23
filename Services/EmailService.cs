using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPRN212.Services
{
    public class EmailService
    {
        private readonly string smtpServer;
        private readonly int port;
        private readonly string fromEmail;
        private readonly string password;

        public EmailService(string smtpServer, int port, string fromEmail, string password)
        {
            this.smtpServer = smtpServer;
            this.port = port;
            this.fromEmail = fromEmail;
            this.password = password;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(fromEmail);
            var toAddress = new MailAddress(toEmail);

            using (var smtp = new SmtpClient(smtpServer, port))
            {
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(fromEmail, password);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}
