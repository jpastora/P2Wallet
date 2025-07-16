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
            var enviado = await _emailService.EnviarOTPAsync(email);
            if (enviado)
                return Ok(new { message = "OTP enviada correctamente. Revisa tu correo." });
            else
                return StatusCode(500, new { message = "No se pudo enviar el OTP. Intenta más tarde." });
        }

        [HttpPost("verificar-otp")]
        public IActionResult VerificarOTP([FromQuery] string email, [FromQuery] string otp)
        {
            if (_emailService.TryGetOTP(email, out var otpGuardado))
            {
                if (otpGuardado == otp)
                {
                    return Ok(new { valido = true, message = "OTP verificado con éxito." });
                }
                return BadRequest(new { valido = false, message = "OTP incorrecto." });
            }
            return BadRequest(new { valido = false, message = "OTP expirado o no encontrado." });
        }
    }
}
