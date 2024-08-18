using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

namespace _2024FinalYearProject.Data
{
    public class ChargesRepository : RepositoryBase<Charges>, IChargesRepository

    {
        public ChargesRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
