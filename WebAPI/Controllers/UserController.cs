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
                return Ok("User created successfully.");
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }





        }

    }
}
