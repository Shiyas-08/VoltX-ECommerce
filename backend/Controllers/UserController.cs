using ECommerce.Models;
using ECommerce.Services;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Policy = "AdminOnly")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async  Task<ActionResult> GetAll()   
        {
            return Ok(await _service.GetAllUser());
          }

        [HttpGet("{id}")]

        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _service.GetById(id);
            if (user == null) return NotFound("User not Found");
            return Ok(user);

        }

        [HttpPut("toggle-block/{id}")]
        public async Task<IActionResult> ToggleBlock(int id)
        {
            var result = await _service.ToggleBlock(id);

            if (!result)
                return NotFound(new { IsSuccess = false, Message = "User not found" });

            return Ok(new { IsSuccess = true, Message = "User block status changed" });
        }


        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(int id)
        //{
        //    var result = await _service.DeleteUser(id);

        //    if (!result)
        //        return NotFound(new { IsSuccess = false, Message = "User not found" });

        //    return Ok(new { IsSuccess = true, Message = "User deleted successfully" });
        //}


    }
}
