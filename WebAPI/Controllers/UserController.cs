using CoreApp;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


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
                userManager.CreateUser(user);
                // Retornar un objeto JSON con message, icon y title
                return Ok(new   
                {
                    message = "Usuario creado exitosamente.",
                    icon = "success",
                    title = "¡Éxito!"
                });
            }
            catch (Exception ex)
            {
                // Retornar un objeto JSON con message, icon y title para error
                return StatusCode(500, new
                {
                    message = ex.Message,
                    icon = "error",
                    title = "Error"
                });
            }
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult UpdateUser(User user)
        {
            try
            {
                var userManager = new UserManager();
                userManager.UpdateUser(user);
                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

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
                    return NotFound("User not found.");
                }
                return Ok(userResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

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
                    return NotFound("User not found.");
                }
                return Ok(userResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}