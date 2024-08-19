using Microsoft.AspNetCore.Identity;

namespace _2024FinalYearProject.Models
{
    public class AppUser : IdentityUser
    {
        public DateOnly DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentStaffNumber { get; set; }
        public string IDnumber { get; set; }
        public string UserRole { get; set; }
    }
}
