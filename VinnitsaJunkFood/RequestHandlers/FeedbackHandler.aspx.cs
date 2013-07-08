using System;
using System.Net;
using System.Net.Mail;
namespace VinnitsaJunkFood.RequestHandlers
{
    public partial class FeedbackHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

       /// <summary>
       /// Sends an mail message
       /// </summary>
       /// <param name="from">Sender address</param>
       /// <param name="to">Recepient address</param>
       /// <param name="bcc">Bcc recepient</param>
       /// <param name="cc">Cc recepient</param>
       /// <param name="subject">Subject of mail message</param>
       /// <param name="body">Body of mail message</param>
       public static string SendMailMessage(string from, string to, string bcc, string cc, string subject, string body){
          // Instantiate a new instance of MailMessage
          MailMessage mMailMessage = new MailMessage();

          // Set the sender address of the mail message
          mMailMessage.From = new MailAddress(from);
          // Set the recepient address of the mail message
          mMailMessage.To.Add(new MailAddress(to));
          
          mMailMessage.Subject = subject;
          // Set the body of the mail message
          mMailMessage.Body = body;

          // Set the format of the mail message body as HTML
          mMailMessage.IsBodyHtml = true;
          // Set the priority of the mail message to normal
          mMailMessage.Priority = MailPriority.Normal;

          // Instantiate a new instance of SmtpClient
          SmtpClient mSmtpClient = new SmtpClient("smtp.mail.ru", 2525){
                                                                                Credentials = new NetworkCredential("junkfood.notification@mail.ru", "collider1!"),
                                                                                Timeout = 10000
                                                                         };          
          string status = "";
          try{
              mSmtpClient.Send(mMailMessage);              
              status = "success";
          }
          catch (SmtpException smtpEx ){
              status = "Fail";
          }
          catch {
              status = "Fail";
          }

          mMailMessage.Dispose();
          mSmtpClient.Dispose();
          return status;          

       }

    }    
}