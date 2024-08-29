using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;
using _2024FinalYearProject.Models.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace _2024FinalYearProject.Data
{
    public class BankAccountRepository : RepositoryBase<BankAccount>, IBankAccountRepository
    {
        private readonly AppDbContext _context;

        public BankAccountRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
