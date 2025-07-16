using DataAccess.ServicesAccess;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public MailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarOTP([FromQuery] string email)
        {
            await _emailService.EnviarOTPAsync(email);
            return Ok("OTP enviado correctamente");
        }

        [HttpPost("verificar-otp")]
        public IActionResult VerificarOTP([FromQuery] string email, [FromQuery] string otp)
        {
            // Recuperar el OTP de la cache y comparar
            if (_emailService.TryGetOTP(email, out var otpGuardado))
            {
                if (otpGuardado == otp)
                {
                    return Ok(new { valido = true });
                }
                return BadRequest(new { valido = false, mensaje = "OTP incorrecto" });
            }
            return BadRequest(new { valido = false, mensaje = "OTP expirado o no encontrado" });
        }
    }
}
