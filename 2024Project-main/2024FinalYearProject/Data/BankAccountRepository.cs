using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

namespace _2024FinalYearProject.Data
{
    public class BankAccountRepository : RepositoryBase<BankAccount>, IBankAccountRepository
    {

        public BankAccountRepository(AppDbContext context) : base(context)
        {
        }
    }
}
