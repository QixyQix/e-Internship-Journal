using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.IO;

namespace E_Internship_Journal.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.


            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
        public Task SendChangeEmailAsync(Boolean button, string email, string subject, string greetings, string message, string url, string urlButtonName)
        {
            try
            {

                //From Address    
                string FromAddress = "admin@sp.edu.sg";
                string FromAdressTitle = "ADMIN @ SP";
                //To Address    
                string ToAddress = email;
                string ToAdressTitle = "e-Internship Journal";
                string Subject = subject;
                //string BodyContent = message;

                //Smtp Server    
                string SmtpServer = "smtp.gmail.com";
                //Smtp Port Number    
                int SmtpPortNumber = 465;

                var mimeMessage = new MimeMessage();
                var bodyBuilder = new BodyBuilder();
                string filePath = System.IO.Path.GetFullPath("wwwroot/images/Email_Templates/GeneralTemplate.html");
                using (StreamReader SourceReader = System.IO.File.OpenText(filePath))
                {
                    string text = SourceReader.ReadToEnd();
                    var qq = "";
                    if (button == false)
                    {
                        qq = text.Replace("display: inline-block;\" class=\"mobile-button\"", "display: none;\" class=\"mobile-button\"");
                    }
                    else
                    {
                        qq = text.Replace("href=\"\">Activate Account</a>", "href=\"" + url + "\">" + urlButtonName + "</a>");
                    }
                    qq = qq.Replace("Header</td>", subject + "</td>");
                    qq = qq.Replace("Greetings</td>", greetings + "</td>");
                    qq = qq.Replace("Body</td>", message + "</td>");
                    //   var qq = text.Replace("href=\"\">Activate Account</a>", "href=\"" + message + "\">Activate Account</a>");
                    //bodyBuilder.HtmlBody = SourceReader.ReadToEnd();
                    bodyBuilder.HtmlBody = qq;
                }

                //// bodyBuilder.HtmlBody = "<h1>Hello, World!</h1>";
                mimeMessage.From.Add(new MailboxAddress
                                        (FromAdressTitle,
                                         FromAddress
                                         ));
                mimeMessage.To.Add(new MailboxAddress
                                         (ToAdressTitle,
                                         ToAddress
                                         ));
                mimeMessage.Subject = Subject; //Subject  
                mimeMessage.Body = bodyBuilder.ToMessageBody();
                //mimeMessage.Body = new TextPart("plain")
                //{
                //    Text = BodyContent
                //};
                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, true);
                    client.Authenticate(
                        "yoshifumiprovidence6@gmail.com",
                        "85112904"
                        );
                    client.Send(mimeMessage);

                    client.Disconnect(true);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Task.FromResult(0);
        }
    }
}
