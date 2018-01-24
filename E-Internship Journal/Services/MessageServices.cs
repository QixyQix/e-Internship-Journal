﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

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
        public Task SendChangeEmailAsync(string email, string subject, string message)
        {
            try
            {
                //From Address    
                string FromAddress = "myname@company.com";
                string FromAdressTitle = "My Name";
                //To Address    
                string ToAddress = email;
                string ToAdressTitle = "Microsoft ASP.NET Core";
                string Subject = subject;
                string BodyContent = message;

                //Smtp Server    
                string SmtpServer = "smtp.gmail.com";
                //Smtp Port Number    
                int SmtpPortNumber = 465;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress
                                        (FromAdressTitle,
                                         FromAddress
                                         ));
                mimeMessage.To.Add(new MailboxAddress
                                         (ToAdressTitle,
                                         ToAddress
                                         ));
                mimeMessage.Subject = Subject; //Subject  
                mimeMessage.Body = new TextPart("plain")
                {
                    Text = BodyContent
                };
                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, true);
                    client.Authenticate(
                        "yoshifumiprovidence5@gmail.com",
                        "chia85112904"
                        );
                    client.SendAsync(mimeMessage);

                    client.DisconnectAsync(true);
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
