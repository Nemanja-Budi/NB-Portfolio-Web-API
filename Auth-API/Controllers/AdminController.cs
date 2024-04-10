using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Auth_API.Models.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using Auth_API.Models.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace Auth_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet("get-members")]
        public async Task<ActionResult<IEnumerable<MemberViewDto>>> GetMembers()
        {
            var members = await userManager.Users
                .Where(x => x.UserName != SD.AdminUserName)
                .Select(member => new MemberViewDto
                { 
                    Id = member.Id,
                    UserName = member.UserName,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    DateCreated =  member.DataCreated,
                    IsLocked = userManager.IsLockedOutAsync(member).GetAwaiter().GetResult(),
                    Roles = userManager.GetRolesAsync(member).GetAwaiter().GetResult()
                }).ToListAsync();

            return  Ok(members);
        }

        [HttpPut("lock-member/{id}")]
        public async Task<IActionResult> LockMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            
            if(user == null) return NotFound();

            if(IsAdminUserId(id))
            {
                return BadRequest(SD.SuperAdminChangeNotAllowed);
            }

            await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));

            return NoContent();
        }

        private bool IsAdminUserId(string userId) 
        { 
            return userManager.FindByIdAsync(userId).GetAwaiter().GetResult().UserName.Equals(SD.AdminUserName);
        }

        [HttpPut("unlock-member/{id}")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            
            if (user == null) return NotFound();

            if (IsAdminUserId(id))
            {
                return BadRequest(SD.SuperAdminChangeNotAllowed);
            }

            await userManager.SetLockoutEndDateAsync(user, null);
            
            return NoContent();
        }

    }
}
