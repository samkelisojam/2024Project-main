using System.ComponentModel.DataAnnotations.Schema;

namespace _2024FinalYearProject.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }

        [ForeignKey(nameof(AppUserId))]
        public string AppUserId { get; set; } // Foreign key to AppUser
        public AppUser AppUser { get; set; }
    }

}
