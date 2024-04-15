﻿using System.ComponentModel.DataAnnotations;

namespace Auth_API.Models.DTOs.Account
{
    public class MemberAddEditDto
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string Password { get; set; }

        [Required]
        public string Roles { get; set; }


    }
}
