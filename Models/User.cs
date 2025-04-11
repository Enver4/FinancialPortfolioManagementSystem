using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentPortfolioAPI.Models
{
    public class User
    {
         public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // store hashed password
        public string Role { get; set; } = "User"; // default role: User or Admin
    }
}