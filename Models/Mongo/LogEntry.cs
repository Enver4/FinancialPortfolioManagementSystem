using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentPortfolioAPI.Models.Mongo
{
    public class LogEntry
    {
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = "General"; // e.g. Upload, Error, Transaction
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public object? Data { get; set; } // optional extra info
    }
}