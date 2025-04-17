using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentPortfolioAPI.Models
{
    public class InvestmentPortfolio
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; } // Optional, for EF navigation

        public int AssetTypeId { get; set; }
        public Currency? AssetType { get; set; }

        public string Currency { get; set; } = "USD";

        public decimal CostBasis { get; set; } // What user originally paid
    }
}