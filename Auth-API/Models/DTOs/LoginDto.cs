using System.ComponentModel.DataAnnotations;

namespace Auth_API.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
