using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;

namespace DataAccess.ServicesAccess
{
    public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public SmsService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"]
                ?? throw new InvalidOperationException("Configure 'Twilio:AccountSid' en appsettings o variables de entorno.");
            _authToken = configuration["Twilio:AuthToken"]
                ?? throw new InvalidOperationException("Configure 'Twilio:AuthToken' en appsettings o variables de entorno.");
            _fromNumber = configuration["Twilio:FromNumber"]
                ?? throw new InvalidOperationException("Configure 'Twilio:FromNumber' en appsettings o variables de entorno.");

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task EnviarOtpAsync(string numeroDestino, string codigoOtp)
        {
            var mensaje = $"Tu código OTP es: {codigoOtp}";

            await MessageResource.CreateAsync(
                body: mensaje,
                from: new Twilio.Types.PhoneNumber(_fromNumber),
                to: new Twilio.Types.PhoneNumber(numeroDestino)
            );
        }
    }
}
