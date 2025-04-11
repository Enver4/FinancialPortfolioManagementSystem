using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using InvestmentPortfolioAPI.Models.Mongo;

namespace InvestmentPortfolioAPI.Services
{
    public class MongoLoggerService
    {
        private readonly IMongoCollection<LogEntry> _logs;

        public MongoLoggerService(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _logs = database.GetCollection<LogEntry>("Logs");
        }

        public async Task LogAsync(LogEntry entry)
        {
            await _logs.InsertOneAsync(entry);
        }
    }
}