using CoreApp;
using DTOs;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public ActionResult CreateUser(User user)
        {
            try
            {
                var userManager = new UserManager();
                user.EmailVerified = "Active"; // Marcar como verificado por OTP
                userManager.CreateUser(user);

                return Ok(new
                {
                    message = "Usuario creado exitosamente.",
                    icon = "success",
                    title = "¡Éxito!",
                    emailVerified = user.EmailVerified
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpPut]
        [Route("Update")]
        public ActionResult UpdateUser(User user)
        {
            try
            {
                var userManager = new UserManager();
                userManager.UpdateUser(user);

                return Ok(new
                {
                    message = "Usuario actualizado correctamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpPut]
        [Route("UpdateBiometricInfo")]
        public ActionResult UpdateBiometricInfo(User user)
        {
            try
            {
                var userManager = new UserManager();
                userManager.UpdateBiometric(user);

                return Ok(new
                {
                    message = "Información Biométrica verificada exitosamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult RetrieveAllUsers()
        {
            try
            {
                var userManager = new UserManager();
                var listUserResult = userManager.RetrieveAllUsers();

                return Ok(new
                {
                    message = "Usuarios recuperados exitosamente.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = listUserResult
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet]
        [Route("RetrieveById/{id}")]
        public ActionResult RetrieveUserById(int id)
        {
            try
            {
                var userManager = new UserManager();
                var userResult = userManager.RetrieveUserById(id);

                if (userResult == null)
                {
                    return NotFound(new
                    {
                        message = "Usuario no encontrado.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Usuario encontrado.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = userResult
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpGet]
        [Route("RetrieveByEmail/{email}")]
        public ActionResult RetrieveUserByEmail(string email)
        {
            try
            {
                var userManager = new UserManager();
                var userResult = userManager.RetrieveUserByEmail(new User { Email = email });

                if (userResult == null)
                {
                    return NotFound(new
                    {
                        message = "Usuario no encontrado.",
                        icon = "warning",
                        title = "Aviso"
                    });
                }

                return Ok(new
                {
                    message = "Usuario encontrado.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = userResult
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult DeleteUser(User user)
        {
            try
            {
                var userManager = new UserManager();
                userManager.DeleteUser(user);

                return Ok(new
                {
                    message = "Usuario eliminado correctamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login([FromBody] JsonElement loginData)
        {
            try
            {
                if (!loginData.TryGetProperty("email", out JsonElement emailElement) || string.IsNullOrWhiteSpace(emailElement.GetString()))
                    return BadRequest(new { message = "El campo 'email' es obligatorio y no puede estar vacío." });

                if (!loginData.TryGetProperty("password", out JsonElement passwordElement) || string.IsNullOrWhiteSpace(passwordElement.GetString()))
                    return BadRequest(new { message = "El campo 'password' es obligatorio y no puede estar vacío." });

                var email = emailElement.GetString();
                var password = passwordElement.GetString();
                var user = new UserManager().RetrieveUserByEmail(new User { Email = email });

                if (user == null || !PasswordHelper.VerifyPassword(password, user.Password))
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos." });

                user.Password = null;

                return Ok(new
                {
                    message = "Inicio de sesión exitoso.",
                    icon = "success",
                    title = "¡Éxito!",
                    data = user
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpPost]
        [Route("ChangePassword")]
        public ActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userManager = new UserManager();
                userManager.ChangeUserPassword(request.UserId, request.NewPassword);

                return Ok(new
                {
                    message = "Contraseña cambiada correctamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }
    }

    public class ChangePasswordRequest
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
