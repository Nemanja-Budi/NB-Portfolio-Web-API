using System.ComponentModel.DataAnnotations;

namespace Auth_API.Models.DTOs.Contact
{
    public class ContactDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "The First Name is required")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name is required")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "The Name of Company is required")]
        public required string NameOfCompany { get; set; }

        [Required(ErrorMessage = "The Email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "The Message is required")]
        public required string Message { get; set; }
    }
}
