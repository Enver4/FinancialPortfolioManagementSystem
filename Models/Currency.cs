using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentPortfolioAPI.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // TRY, USD etc
    }
}