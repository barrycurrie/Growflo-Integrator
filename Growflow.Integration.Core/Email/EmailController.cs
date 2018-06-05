using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Email
{
    public class EmailController
    {
        private readonly string _smtpServer;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly int _smtpPort;

        public EmailController(string smtpServer, string smtpUsername, string smtpPassword, int smtpPort)
        {
            _smtpServer = smtpServer;
            _smtpPassword = smtpPassword;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
        }

        public void Send(string to, string from, string subject, string body,params string[] attachments)
        {
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        if (!string.IsNullOrWhiteSpace(attachment) && System.IO.File.Exists(attachment))
                        {
                            mailMessage.Attachments.Add(new Attachment(attachment));
                        }
                    }
                }

                var smtpClient = new SmtpClient(_smtpServer, _smtpPort);
                smtpClient.Port = _smtpPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(_smtpUsername, _smtpPassword);
                smtpClient.EnableSsl = false;

                smtpClient.Send(mailMessage);
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured sending an email: " + ex.Message, ex);
            }
        }
    }
}
