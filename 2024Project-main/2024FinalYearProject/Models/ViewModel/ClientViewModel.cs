namespace _2024FinalYearProject.Models
{
    public class BankAccountViewModel
    {
        public IEnumerable<BankAccount> BankAccount { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
    public class CashSentViewModel
    {
        public int BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal AvailableBalance { get; set; }  // Add this property
    }



}
