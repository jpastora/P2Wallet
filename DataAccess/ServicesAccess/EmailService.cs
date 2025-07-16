using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DataAccess.ServicesAccess
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "joe.red.pruebas@gmail.com";
        private readonly string _smtpPass = "wqdz blov cnvz vlrl";
        private readonly IMemoryCache _cache;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IMemoryCache cache, ILogger<EmailService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress("Yavi", _smtpUser));
                mensaje.To.Add(MailboxAddress.Parse(destinatario));
                mensaje.Subject = asunto;

                var bodyBuilder = new BodyBuilder { HtmlBody = cuerpoHtml };
                mensaje.Body = bodyBuilder.ToMessageBody();

                using var cliente = new SmtpClient();
                await cliente.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                await cliente.AuthenticateAsync(_smtpUser, _smtpPass);
                await cliente.SendAsync(mensaje);
                await cliente.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando correo a {destinatario}");
                return false;
            }
        }

        public async Task<bool> EnviarOTPAsync(string destinatario)
        {
            var codigoOTP = new Random().Next(100000, 999999).ToString();
            var html = $"<h3>Tu código OTP es: <strong>{codigoOTP}</strong></h3>";

            var enviado = await EnviarCorreoAsync(destinatario, "Tu código de verificación", html);
            if (enviado)
            {
                _cache.Set($"OTP_{destinatario}", codigoOTP, TimeSpan.FromMinutes(5));
            }
            return enviado;
        }

        public bool TryGetOTP(string destinatario, out string otp)
        {
            return _cache.TryGetValue($"OTP_{destinatario}", out otp);
        }
    }
}
