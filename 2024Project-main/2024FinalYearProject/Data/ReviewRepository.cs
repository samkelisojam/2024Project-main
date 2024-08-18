using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

namespace _2024FinalYearProject.Data
{
    public class ReviewRepository : RepositoryBase<FeedBack>, IReviewRepository
    {
        public ReviewRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
