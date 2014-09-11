using System.Net;
using System.Net.Mail;

namespace VinnitsaJunkFood.BusinessLayer.Helpers
{
    public static class FeedbackHelper{
        /// <summary>
        /// Sends an mail message
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recepient address</param>        
        /// <param name="subject">Subject of mail message</param>
        /// <param name="body">Body of mail message</param>
        public static bool SendMailMessage(string from, string to, string subject, string body) {
            bool  success = false;
            var mailMessage = new MailMessage  {
                                                            From = new MailAddress(from),
                                                            Subject = subject,
                                                            Body = body,
                                                            IsBodyHtml = true,
                                                            Priority = MailPriority.Normal
                                                        };
            mailMessage.To.Add(new MailAddress(to));            

            // Instantiate a new instance of SmtpClient
            var mSmtpClient = new SmtpClient("smtp.mail.ru", 2525)   {
                                                                                Credentials = new NetworkCredential("junkfood.notification@mail.ru", "freedom1!"),
                                                                                Timeout = 10000
                                                                            };
            

            try {
                mSmtpClient.Send(mailMessage);                
                success = true;
            }
            catch (SmtpException){}
            finally{
                mailMessage.Dispose();
                mSmtpClient.Dispose();
            }            

            return success;
        }

    }
}