using System.Net;
using System.Net.Mail;
using System.Text;

namespace KleinLibrary.Emails
{
    public sealed class SmtpController
    {
        /// <summary>
        /// Die Anmeldedaten für den SMTP Server
        /// </summary>
        public SmtpConfig Configuration { get; set; }
        /// <summary>
        /// Erstellt eine neue Instanz des <see cref="SmtpController"/> mit den angegebenen Credentials
        /// </summary>
        /// <param name="credentials">Die Credentials die zur Anmeldung genutzt werden sollen</param>
        public SmtpController(SmtpConfig config)
        {
            Configuration = config;
        }
        public Task<EmailResponse> SendenAsync(Email email)
        {
            using MailMessage msg = new MailMessage();
            msg.IsBodyHtml = email.HtmlMail;
            foreach (string empfänger in email.Empfänger)
            {
                msg.To.Add(empfänger);
            }

            foreach (string ccMail in email.CC)
            {
                msg.CC.Add(ccMail);
            }

            foreach (string bccMail in email.BCC)
            {
                msg.Bcc.Add(bccMail);
            }



            msg.Subject = email.Betreff;
            msg.Body = email.Inhalt;
            msg.BodyEncoding = Encoding.UTF8;
            msg.From = new MailAddress(Configuration.AbsenderEmail, Configuration.AbsenderName); ;
            msg.ReplyToList.Add(new MailAddress(Configuration.ReplyTo, Configuration.AbsenderName));


            foreach (EmailAnhang anhang in email.Anhänge)
            {
                try
                {
                    StreamReader reader = new StreamReader(anhang.Pfad);
                    Attachment attach = new Attachment(reader.BaseStream, anhang.Name);
                    msg.Attachments.Add(attach);
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new EmailResponse(false, $"#EMAIL3 - SMTP({Configuration.Host}): {ex}"));
                }
            }


            using SmtpClient smtp = new SmtpClient(Configuration.Host, Configuration.Port);
            NetworkCredential credential = new NetworkCredential(Configuration.Username, Configuration.Password);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = credential;
            smtp.EnableSsl = Configuration.SSL;

            try
            {
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                string message = $"#EMAIL2 - SMTP({Configuration.Host}) Fehler: {ex}";
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                {
                    message += $" | {ex.InnerException.Message}";
                }
                return Task.FromResult(new EmailResponse(false, message));
            }

            return Task.FromResult(new EmailResponse(true, "OK"));
        }
    }
}
