using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2024FinalYearProject.Models.ViewModels
{
    public class ConsultantViewModel
    {
        public IQueryable<AppUser> appUsers { get; set; }
        public IEnumerable<LoginSessions> loginSessions { get; set; }
        public AppUser SelectedUser { get; set; }
    }
}