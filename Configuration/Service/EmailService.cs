﻿using Microsoft.Extensions.Options;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Configuration.IService;
using System.Net;
using System.Net.Mail;

namespace Spider_EMT.Configuration.Service
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSetting> _smtpSetting;
        public EmailService(IOptions<SmtpSetting> smtpSetting)
        {
            _smtpSetting = smtpSetting;
        }
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);
            using (var emailClient = new SmtpClient(_smtpSetting.Value.Host, _smtpSetting.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(_smtpSetting.Value.User, _smtpSetting.Value.Password);
                await emailClient.SendMailAsync(message);
            }
        }
    }
}
