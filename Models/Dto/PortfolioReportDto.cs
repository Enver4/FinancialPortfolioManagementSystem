using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentPortfolioAPI.Models.Dto
{
    public class PortfolioReportDto
    {
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public decimal Amount { get; set; }
        public decimal CostBasis { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal ProfitLoss => CurrentValue - CostBasis;
    }
}