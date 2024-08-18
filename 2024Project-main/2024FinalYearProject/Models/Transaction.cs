using System.ComponentModel.DataAnnotations.Schema;

namespace _2024FinalYearProject.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int BankAccountIdSender { get; set; } 
        public int BankAccountIdReceiver { get; set; } 

        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
      
        public string Reference { get; set; }

        //Nav property
        [ForeignKey(nameof(AppUserId))]
        public string AppUserId { get; set; } 
        public AppUser AppUser { get; set; }
       
    }

}
