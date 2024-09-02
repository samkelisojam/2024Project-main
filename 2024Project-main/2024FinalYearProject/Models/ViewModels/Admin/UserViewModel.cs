namespace _2024FinalYearProject.Models.ViewModels.Admin
{
    public class UserViewModel
    {
        public AppUser AppUser { get; set; }
        public BankAccount BankAccount { get; set; }
        public string _fullName { get; set; } = string.Empty;
    }
}
