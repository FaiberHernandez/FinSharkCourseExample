using api.Dtos.Account;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);

                var newUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);

                if(createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
                    if(roleResult.Succeeded)
                    {
                        return Ok("User created successfully");
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
                
            }
            catch (Exception e)
            {
                
                return StatusCode(500, e);
            }
        }
    }
}