namespace _2024FinalYearProject.Models.ViewModels.Admin
{
    public class IndexPageViewModel
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<AppUser> Consultants { get; set; } = new List<AppUser>();
        public List<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
