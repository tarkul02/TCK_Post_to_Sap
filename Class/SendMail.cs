using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace SAP_Batch_GR_TR.Class
{
    class SendMail
    {
        internal void FNSendMail1()
        {
            SendMailClass();
        }

        public void SendMailClass()
        {
            const string ToAddress = "Beerbeerlovemusic@gmail.com";
            const string FromAddress = "tarkulbeer@gmail.com";

            const string GoogleAppPassword = "ajdw rtsh wcqu kooh";

            const string EmailSubject = "Test email!!222333";
            const string EmailBody = "<h1>Hi</h1>";

            Console.WriteLine("Hello World!");
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(FromAddress, GoogleAppPassword),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(FromAddress),
                    Subject = EmailSubject,
                    Body = EmailBody,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(ToAddress);

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
