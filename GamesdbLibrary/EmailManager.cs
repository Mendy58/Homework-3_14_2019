using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GamesdbLibrary
{
    public class EmailManager
    {
        //HAhaha I realized they could both have been one fun :) Sorry, feel to sorry for myself to delete one...
        public void SendNotification(EmailContent content)
        {
              string mailBodyhtml = $"{content.message}";
              var msg = new MailMessage("mendelstest@gmail.com", $"{content.To}", $"{content.Subject}", mailBodyhtml);
              msg.To.Add("to2@gmail.com");
              msg.IsBodyHtml = true;
              var smtpClient = new SmtpClient("smtp.gmail.com", 587);
              smtpClient.UseDefaultCredentials = true;
              smtpClient.Credentials = new NetworkCredential("mendelstest@gmail.com", "Imnotsaying2019");
              smtpClient.EnableSsl = true;
              smtpClient.Send(msg);

              Console.ReadKey(true);
        }
        public void SendNotificationToMultiple(EmailContent content, List<string> CCs)
        {
            string mailBodyhtml = $"{content.message}";
            var msg = new MailMessage("mendelstest@gmail.com", $"{content.To}", $"{content.Subject}", mailBodyhtml);
            foreach(string s in CCs)
            {
                msg.Bcc.Add(s);
            }
            msg.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new NetworkCredential("mendelstest@gmail.com", "Imnotsaying2019");
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);

            Console.ReadKey(true);
        }
    }
}
