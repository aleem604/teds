using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Models
{

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class SignupViewModel
    {
        public string Id { get; set; }
        [Required, MinLength(4)]
        public string FirstName { get; set; }
        [Required, MinLength(4)]
        public string LastName { get; set; }
        [Required, MinLength(4), EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(5)]
        public string Password { get; set; }

    }

    public class ChangePasswordViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }


}
