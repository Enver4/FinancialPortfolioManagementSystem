using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentPortfolioAPI.Models
{
    public class AssetType
    {
         public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., Cash, Stock, Crypto, Gold
    }
}