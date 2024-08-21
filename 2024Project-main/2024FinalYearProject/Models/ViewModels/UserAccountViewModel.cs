using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace _2024FinalYearProject.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [UIHint("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [UIHint("password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }

    public class RegisterViewModel
    {
        public string RegisterAs { get; set; } = "studentstaff";

        [Required(ErrorMessage = "Please enter a unique email address")]
        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter ID or passport number.")]
        [DisplayName("ID or Passport number")]
        public string IdPassportNumber { get; set; }

        [DisplayName("Student or Staff number")]
        public string StudentStaffNumber { get; set; }


        [Required(ErrorMessage = "Please enter first name")]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [DisplayName("Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateProfileViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string AccountNumber { get; set; }
        public string IDNumber { get; set; }
        public string Userrole { get; set; }
    }
}
