using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestmentPortfolioAPI.Models.Dto;
using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using InvestmentPortfolioAPI.Data;
using InvestmentPortfolioAPI.Models;
using Microsoft.Extensions.Configuration;
namespace InvestmentPortfolioAPI.Services
{
    public class ExchangeRateUpdater
    {
        private readonly HttpClient _http;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public ExchangeRateUpdater(HttpClient http, ApplicationDbContext db)
        {
            _http = http;
            _db = db;
        }
        
        public async Task UpdateRatesAsync(string baseCurrency = "USD")
        {
            Console.WriteLine($"🌐 Fetching rates for {baseCurrency}...");
            var url = $"https://api.frankfurter.dev/v1/latest?base={baseCurrency}";
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                Console.WriteLine($" API failed for {baseCurrency} - {response.StatusCode}");
                //

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($" API response (truncated): {content[..Math.Min(content.Length, 200)]}");

            var data = JsonSerializer.Deserialize<ExchangeRateApiResponse>(content);

            if (data == null || data.Rates == null){
                Console.WriteLine($" Could not parse exchange rate data for {baseCurrency}");
                return;
            }

            var targetCurrencies = new[] { "USD", "EUR", "TRY", "GBP" };

            foreach (var target in targetCurrencies)
            {
                if (target == baseCurrency) continue;

                if (data.Rates.TryGetValue(target, out decimal rate))
                {
                    var existing = await _db.ExchangeRates
                        .FirstOrDefaultAsync(r => r.FromCurrency == baseCurrency && r.ToCurrency == target);

                    //if (existing == null)
                    //{
                        Console.WriteLine($" Adding new rate: {baseCurrency} ➜ {target} = {rate}");

                        _db.ExchangeRates.Add(new ExchangeRate
                        {
                            FromCurrency = baseCurrency,
                            ToCurrency = target,
                            Rate = rate,
                            UpdatedAt = DateTime.UtcNow
                        });
                    //}
                    /*else
                    {
                        Console.WriteLine($" Updating rate: {baseCurrency} ➜ {target} = {rate}");
                        existing.Rate = rate;
                        existing.UpdatedAt = DateTime.UtcNow;
                    }*/
                    
                }
                    
                    else{
                        Console.WriteLine($" Rate not found for {baseCurrency} ➜ {target}");
                        }
            }
            
            await _db.SaveChangesAsync();
        }
    }
}