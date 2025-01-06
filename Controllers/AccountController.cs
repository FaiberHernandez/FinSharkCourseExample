using api.Dtos.Account;
using api.interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

            if(user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded) return Unauthorized("Invalid username and/or password");

            return Ok(new NewUserDto
            {
                UserName = user.UserName!,
                Email = user.Email!,
                Token = _tokenService.CreateToken(user)
            });
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

                if(!createdUser.Succeeded) return StatusCode(500, createdUser.Errors);

                var roleResult = await _userManager.AddToRoleAsync(newUser, "User");

                if(!roleResult.Succeeded) return StatusCode(500, roleResult.Errors);
                
                return Ok(new NewUserDto
                {
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    Token = _tokenService.CreateToken(newUser)
                });
                
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}