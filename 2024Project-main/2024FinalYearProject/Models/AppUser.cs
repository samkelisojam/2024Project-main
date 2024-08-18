using Microsoft.AspNetCore.Identity;

namespace _2024FinalYearProject.Models
{
    public class AppUser : IdentityUser
    {
        public string IDnumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
