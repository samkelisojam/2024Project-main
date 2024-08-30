using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2024FinalYearProject.Models
{
    public class LoginSessions
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}