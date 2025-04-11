using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentPortfolioAPI.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string FromCurrency { get; set; } = string.Empty; // e.g., "USD"
        public string ToCurrency { get; set; } = string.Empty;   // e.g., "EUR"
        public decimal Rate { get; set; }                        // e.g., 0.92
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}