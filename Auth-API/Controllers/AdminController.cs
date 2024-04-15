using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Auth_API.Models.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;
using Auth_API.Models.DTOs.Account;

namespace Auth_API.Controllers
{
    [Authorize(Roles = "Admin")]
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

        [HttpGet("get-member/{id}")]
        public async Task<ActionResult<MemberAddEditDto>> GetMember(string id) 
        {
            var member = await userManager.Users
                .Where(x => x.UserName != SD.AdminUserName && x.Id == id)
                .Select(m => new MemberAddEditDto 
                { 
                    Id = m.Id,
                    UserName = m.UserName,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Roles = string.Join(",", userManager.GetRolesAsync(m).GetAwaiter().GetResult())
                }).FirstOrDefaultAsync();

            return Ok(member);
        }

        [HttpPost("add-edit-member")]
        public async Task<IActionResult> AddEditMember(MemberAddEditDto memberAddEditDto)
        {
            User user;

            if(string.IsNullOrEmpty(memberAddEditDto.Id)) 
            {
                if(string.IsNullOrEmpty(memberAddEditDto.Password) || memberAddEditDto.Password.Length < 6)
                {
                    ModelState.AddModelError("errors", "Password must be at least 6 characters");
                    return BadRequest(ModelState);
                }

                user = new User
                {
                    FirstName = memberAddEditDto.FirstName.ToLower(),
                    LastName = memberAddEditDto.LastName.ToLower(),
                    UserName = memberAddEditDto.UserName.ToLower(),
                    Email = memberAddEditDto.UserName.ToLower(),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, memberAddEditDto.Password);
                if (!result.Succeeded) return BadRequest(result.Errors);
            }
            else
            {
                if(!string.IsNullOrEmpty(memberAddEditDto.Password))
                {
                    if(memberAddEditDto.Password.Length < 6)
                    {
                        ModelState.AddModelError("errors", "Password must be at least 6 characters");
                        return BadRequest(ModelState);
                    }
                }

                if (IsAdminUserId(memberAddEditDto.Id))
                {
                    return BadRequest(SD.SuperAdminChangeNotAllowed);
                }

                user = await userManager.FindByIdAsync(memberAddEditDto.Id);

                if (user == null) return NotFound();
                
                user.FirstName = memberAddEditDto.FirstName.ToLower();
                user.LastName= memberAddEditDto.LastName.ToLower();
                user.UserName= memberAddEditDto.UserName.ToLower();

                if(!string.IsNullOrEmpty(memberAddEditDto.Password))
                {
                    await userManager.RemovePasswordAsync(user);
                    await userManager.AddPasswordAsync(user, memberAddEditDto.Password);
                }
            }

            var userRoles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, userRoles);

            foreach(var role in memberAddEditDto.Roles.Split(",").ToArray())
            {
                var roleToAdd = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role);

                if(roleToAdd != null)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            if(string.IsNullOrEmpty(memberAddEditDto.Id))
            {
                return Ok(new JsonResult(new { title = "Member Created", message = $"{memberAddEditDto.UserName} has been created" }));
            }
            else
            {
                return Ok(new JsonResult(new { title = "Member edited", message = $"{memberAddEditDto.UserName} has been updated" }));
            }
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

        [HttpDelete("delete-member/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            if (IsAdminUserId(id))
            {
                return BadRequest(SD.SuperAdminChangeNotAllowed);
            }

            await userManager.DeleteAsync(user);
            return NoContent();
        }

        [HttpGet("get-application-roles")]
        public async Task<ActionResult<string[]>> GetApplicationRoles()
        {
            return Ok(await roleManager.Roles.Select(x => x.Name).ToListAsync());
        }

        private bool IsAdminUserId(string userId)
        {
            return userManager.FindByIdAsync(userId).GetAwaiter().GetResult().UserName.Equals(SD.AdminUserName);
        }
        
    }
}
