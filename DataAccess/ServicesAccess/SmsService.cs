using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;

namespace DataAccess.ServicesAccess
{
    public class SmsService
    {
        private readonly string _accountSid = "ACb7f405a447d6e2f358d139af231157a0";
        private readonly string _authToken = "9c3df734b089ab904efbe8a8cf884ead";
        private readonly string _fromNumber = "+13513331475"; // Número Twilio

        public SmsService()
        {
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

