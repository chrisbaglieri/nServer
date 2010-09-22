using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;

namespace Server.Messaging
{
    public static class EmailHelper
    {
        /// <summary>
        /// Sends an email message using pre-defined configuration settings.
        /// </summary>
        /// <param name="to">the email address to send to</param>
        /// <param name="from">the email address to send from</param>
        /// <param name="subject">the subject of the mail</param>
        /// <param name="body">the body of the message</param>
        public static void SendMail(string to, string from, string subject, string body)
        {
            try {
                
                // construct email client
                System.Net.Mail.SmtpClient smtpClient = new System.Net.
                    Mail.SmtpClient(((System.Collections.IDictionary) ConfigurationManager.
                    GetSection("emailSettings"))["smtphost"].ToString());

                // assemble mail
                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                email.To.Add(to);
                email.From = new System.Net.Mail.MailAddress(from);
                email.Subject = subject;
                email.Body = body;

                // send mail
                smtpClient.Send(email);

            } catch (Exception ex) {

                // swallow the exception (nom nom nom) and log it
                // you probably don't want to exit just because an 
                // email failed
                Logging.LogHelper.LogMessage(
                    String.Format("Failed to send email to {0}", to),
                    Server.Logging.LogLevel.Error, ex);

            }

        }

    }

}