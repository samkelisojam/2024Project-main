using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

namespace _2024FinalYearProject.Data
{
  
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {

        public NotificationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
