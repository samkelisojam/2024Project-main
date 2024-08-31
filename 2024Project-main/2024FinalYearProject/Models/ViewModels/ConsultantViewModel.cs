namespace _2024FinalYearProject.Models.ViewModels
{
    public class ConsultantViewModel
    {
        public IQueryable<AppUser> appUsers { get; set; }
        public IEnumerable<LoginSessions> loginSessions { get; set; }
        public AppUser SelectedUser { get; set; }
    }

    public class ConsultantDepositModel
    {
        public string UserEmail { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}