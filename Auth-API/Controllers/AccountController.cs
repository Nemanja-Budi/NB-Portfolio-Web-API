using Auth_API.Models.Domain;
using Auth_API.Models.DTOs.Account;
using Auth_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService jWTService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public AccountController(JWTService jWTService, SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {
            this.jWTService = jWTService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            
            if(await userManager.IsLockedOutAsync(user))
            {
                return Unauthorized("You have been locked out");
            }

            return await CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByNameAsync(loginDto.UserName);

            if (user == null) return Unauthorized("Invalid username or password");

            if (user.EmailConfirmed == false) return Unauthorized("Plase confirm your email");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(result.IsLockedOut)
            {
                return Unauthorized(string.Format("Your account has been locked. You should wait until {0} (UTC time) to be able to login", user.LockoutEnd));
            }

            if (!result.Succeeded) 
            {
                if(!user.UserName.Equals(SD.AdminUserName))
                {
                    await userManager.AccessFailedAsync(user);
                }

                if(user.AccessFailedCount >= SD.MaximumLoginAttempts)
                {
                    await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(1));
                    return Unauthorized(string.Format("Your account has been locked. You should wait until {0} (UTC time) to be able to login", user.LockoutEnd));
                }
                return Unauthorized("Invalid username or password");
            }

            await userManager.ResetAccessFailedCountAsync(user);
            await userManager.SetLockoutEndDateAsync(user, null);

            return await CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto) 
        {
            if(await CheckEmailExistsAsync(registerDto.Email))
            {
                return BadRequest($"An existing account is using {registerDto.Email}, email addres. Plase try with another email address");
            }

            var userToAdd = new User
            {
                FirstName = registerDto.FirstName.ToLower(),
                LastName = registerDto.LastName.ToLower(),
                UserName = registerDto.Email.ToLower(),
                Email = registerDto.Email.ToLower(),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(userToAdd, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(userToAdd, SD.PlayerRole);

            try
            {
                if(userToAdd != null)
                {
                    return Ok(new JsonResult(new {title = "Account Created", message = "Your account has been created, please confirm your email address" }));
                }

                return BadRequest("Faild to send email. Please contact admin");
            }
            catch (Exception)
            {
                return BadRequest("Faild to send email. Please contact admin");
            }
        }


        private async Task<UserDto> CreateApplicationUserDto(User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await jWTService.CreateJWT(user),
            };
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

    }
}
