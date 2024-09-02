
using System.ComponentModel.DataAnnotations;

namespace _2024FinalYearProject.Models.ViewModels




{
    public class BankAccountViewModel
    {
        public IEnumerable<BankAccount> BankAccount { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
    public class CashSentViewModel
    {
        public int BankAccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public decimal AvailableBalance { get; set; }  // Add this property
    }
    public class BankAccountViewModela
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string BankAccountType { get; set; }
    }



public class MoneyTransferViewModel
    {
        [Required]
        public int SenderBankAccountId { get; set; }

      
        public int ReceiverBankAccountId { get; set; }

        [Required(ErrorMessage = "The amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The amount must be greater than zero.")]
        public decimal Amount { get; set; }

       
        public string SenderBankAccountNumber { get; set; }

        [Required(ErrorMessage = "The receiver's bank account number is required.")]
      
        public string ReceiverBankAccountNumber { get; set; }

      
        public decimal AvailableBalance { get; set; }
    }


}
