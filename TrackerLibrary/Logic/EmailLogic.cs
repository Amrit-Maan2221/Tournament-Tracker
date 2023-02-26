using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Logic
{
    public static class EmailLogic
    {
        public static void SendEmail(string toAdresses, string subject, string body)
        {
            try
            {
                MailAddress fromMailAdress = new MailAddress(GlobalConfig.AppKeyLookup("senderEmail"), GlobalConfig.AppKeyLookup("senderName"));

                MailMessage mail = new MailMessage();
                mail.To.Add(toAdresses);
                mail.From = fromMailAdress;
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();

                client.Send(mail);
            } catch (Exception ex)
            { 
                // Failed to send the Email
            }

        }
    }
}
