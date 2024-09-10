

namespace _2024FinalYearProject.Models.ViewModels
{
    public class FinAdvisorViewModel
    {
        public AppUser CurrentUser { get; set; }
        public List<Transaction> Transactions { get; set; }
        public BankAccount CurrentUserBankAccount { get; set; }
    }

    public class AdvisorViewModel : FinAdvisorViewModel
    {
        public string UserEmail { get; set; }
        public string Advise { get; set; }
    }
}
