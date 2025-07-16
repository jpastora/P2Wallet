using DataAccess.ServicesAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly SmsService _smsService;
        private readonly IMemoryCache _cache;

        public SmsController(SmsService smsService, IMemoryCache cache)
        {
            _smsService = smsService;
            _cache = cache;
        }

        [HttpPost("enviar-sms")]
        public async Task<IActionResult> EnviarOTPporSms([FromQuery] string telefono)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            await _smsService.EnviarOtpAsync(telefono, otp);
            _cache.Set($"SMS_OTP_{telefono}", otp, TimeSpan.FromMinutes(5));
            return Ok(new { message = "OTP enviado por SMS" });
        }

        [HttpPost("verificar-otp")]
        public IActionResult VerificarSmsOTP([FromQuery] string telefono, [FromQuery] string otp)
        {
            if (_cache.TryGetValue($"SMS_OTP_{telefono}", out string otpGuardado))
            {
                if (otpGuardado == otp)
                {
                    return Ok(new { valido = true, message = "OTP SMS verificado con éxito." });
                }
                return BadRequest(new { valido = false, message = "OTP SMS incorrecto." });
            }
            return BadRequest(new { valido = false, message = "OTP SMS expirado o no encontrado." });
        }
    }
}
