using System.ComponentModel.DataAnnotations;

namespace Auth_API.Models.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
