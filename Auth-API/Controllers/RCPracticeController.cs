using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RCPracticeController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("Public");
        }

        #region Roles
        [HttpGet("admin-role")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminRole()
        {
            return Ok("Admin role");
        }

        [HttpGet("manager-role")]
        [Authorize(Roles = "Manager")]
        public IActionResult ManagerRole()
        {
            return Ok("Manager role");
        }

        [HttpGet("player-role")]
        [Authorize(Roles = "Player")]
        public IActionResult PlayerRole()
        {
            return Ok("Player role");
        }

        [HttpGet("admin-or-manager-role")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult AdminOrManagerRole()
        {
            return Ok("Admin or manager role");
        }

        [HttpGet("admin-or-player-role")]
        [Authorize(Roles = "Admin,Player")]
        public IActionResult AdminOrPlayerRole()
        {
            return Ok("Admin or player role");
        }
        #endregion

        #region Policy

        [HttpGet("admin-policy")]
        [Authorize(policy: "AdminPolicy")]
        public IActionResult AdminPolicy()
        {
            return Ok("Admin policy");
        }

        [HttpGet("manager-policy")]
        [Authorize(policy: "ManagerPolicy")]
        public IActionResult ManagerPolicy()
        {
            return Ok("Manager policy");
        }

        [HttpGet("player-policy")]
        [Authorize(policy: "PlayerPolicy")]
        public IActionResult PlayerPolicy()
        {
            return Ok("Player policy");
        }

        [HttpGet("admin-or-manager-policy")]
        [Authorize(policy: "AdminOrManagerPolicy")]
        public IActionResult AdminOrManagerPolicy()
        {
            return Ok("Admin or manager policy");
        }

        [HttpGet("admin-and-manager-policy")]
        [Authorize(policy: "AdminAndManagerPolicy")]
        public IActionResult AdminAndManagerPolicy()
        {
            return Ok("Admin and manager policy");
        }

        [HttpGet("all-role-policy")]
        [Authorize(policy: "AllRolePolicy")]
        public IActionResult AllRolePolicy()
        {
            return Ok("All role policy");
        }

        #endregion

        #region Claim Policy

        [HttpGet("admin-email-policy")]
        [Authorize(policy: "AdminEmailPolicy")]
        public IActionResult AdminEmailPolicy()
        {
            return Ok("Admin email policy");
        }

        [HttpGet("miller-surname-policy")]
        [Authorize(policy: "MillerSurnamePolicy")]
        public IActionResult MillerSurnamePolicy()
        {
            return Ok("Miller surname policy");
        }

        [HttpGet("manager-email-and-wilson-surname-policy")]
        [Authorize(policy: "ManagerEmailAndWilsonSurnamePolicy")]
        public IActionResult ManagerEmailAndWilsonSurnamePolicy()
        {
            return Ok("Manager email and Wilson surname policy");
        }

        [HttpGet("vip-policy")]
        [Authorize(policy: "VIPPolicy")]
        public IActionResult VIPPolicy()
        {
            return Ok("VIP policy");
        }

        #endregion
    }
}
