using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Crea un nuevo usuario
        [HttpPost]
        [Route("Create")]
        public ActionResult CreateUser(User user)
        {
            try
            {
                var userManager = new UserManager();
                user.EmailVerified = "Active"; // Marcar como verificado por OTP
                userManager.CreateUser(user);
                // Retorna un objeto JSON con mensaje de éxito
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
                // Retorna un objeto JSON con mensaje de error
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        // Actualiza un usuario existente
        [HttpPut]
        [Route("Update")]
        public ActionResult UpdateUser(User user)
        {
            try
            {
                var userManager = new UserManager();
                userManager.UpdateUser(user);
                return Ok(new { message = "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Recupera todos los usuarios
        [HttpGet]
        [Route("RetrieveAll")]
        public ActionResult<List<User>> RetrieveAllUsers()
        {
            try
            {
                var userManager = new UserManager();
                var listUserResult = userManager.RetrieveAllUsers();
                return Ok(listUserResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Recupera un usuario por su ID
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
                    return NotFound("Usuario no encontrado.");
                }
                return Ok(userResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Recupera un usuario por su correo electrónico
        [HttpGet]
        [Route("RetrieveByEmail/{email}")]
        public ActionResult RetrieveUserByEmail(string email)
        {
            try
            {
                var userManager = new UserManager();
                var user = new User { Email = email };
                var userResult = userManager.RetrieveUserByEmail(user);
                if (userResult == null)
                {
                    return NotFound("Usuario no encontrado.");
                }
                return Ok(userResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Elimina un usuario
        [HttpDelete]
        [Route("Delete")]
        public ActionResult DeleteUser(User user)
        {
            try
            {
                var userManager = new UserManager();
                userManager.DeleteUser(user);
                return Ok(new { message = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Inicia sesión de usuario validando solo email y contraseña
        [HttpPost]
        [Route("Login")]
        public ActionResult Login([FromBody] JsonElement loginData)
        {
            try
            {
                // Validate and extract email and password from the body JSON
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
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}