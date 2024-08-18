using System.ComponentModel.DataAnnotations.Schema;

namespace _2024FinalYearProject.Models
{
    public class Charges
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime ChargeDate { get; set; }
        public string Description { get; set; }

        //Nav property 
        [ForeignKey(nameof(AppUserId))]
        public string AppUserId { get; set; } // Foreign key to AppUser
        public AppUser AppUser { get; set; }
    }

}
