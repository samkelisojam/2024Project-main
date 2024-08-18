using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

namespace _2024FinalYearProject.Data
{
  
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository

    {
        public TransactionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
