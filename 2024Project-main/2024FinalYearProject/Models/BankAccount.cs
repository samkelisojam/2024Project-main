using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024FinalYearProject.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string BankAccountType { get; set; }
        public int AccountOrder { get; set; } // control if main account or savings

        public string UserEmail { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }

}
