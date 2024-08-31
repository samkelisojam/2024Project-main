using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

namespace _2024FinalYearProject.Data
{
    public class LoginRepository : RepositoryBase<LoginSessions>, ILoginRepository

    {
        public LoginRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}