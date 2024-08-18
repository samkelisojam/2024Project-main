using System.ComponentModel.DataAnnotations.Schema;

namespace _2024FinalYearProject.Models
{
    public class FeedBack
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime dateTime { get; set; }
        public int Rate { get; set; }

        //Nav property
        [ForeignKey(nameof(AppUserId))]
        public string AppUserId { get; set; }
        AppUser appUser { get; set; }
    }


}
