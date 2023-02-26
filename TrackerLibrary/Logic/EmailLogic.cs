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
        public static void SendEmail(List<string> toAddresses, List<string> bcc, string subject, string body)
        {
            try
            {
                MailAddress fromMailAdress = new MailAddress(GlobalConfig.AppKeyLookup("senderEmail"), GlobalConfig.AppKeyLookup("senderName"));

                MailMessage mail = new MailMessage();
                foreach (string email in toAddresses)
                {
                    mail.To.Add(email);
                }
                foreach (string email in bcc)
                {
                    mail.Bcc.Add(email);
                }
                mail.From = fromMailAdress;
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();

                client.Send(mail);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        public static void SendEmail(string toAddresses, string subject, string body)
        {
            SendEmail(new List<string> { toAddresses }, new List<string>(), subject, body);
        }
    }
}
