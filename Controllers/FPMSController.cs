using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentPortfolioAPI.Data;
using InvestmentPortfolioAPI.Models;
using InvestmentPortfolioAPI.Services;
using System.Text.RegularExpressions;

namespace InvestmentPortfolioAPI.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class FPMSController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FPMSController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExchangeRates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRates()
        {
            return await _context.ExchangeRates.ToListAsync();
        }

        [HttpGet("Latest Rates")]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetLastExchangeRates()
        {
            var latestRates = await _context.ExchangeRates
                .GroupBy(r => new { r.FromCurrency, r.ToCurrency })
                .Select(g => g.OrderByDescending(r => r.UpdatedAt).First())
                .ToListAsync();

            return Ok(latestRates);
        }

        // POST: api/ExchangeRates (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrUpdateRate(ExchangeRate rate)
        {
            var existing = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => r.FromCurrency == rate.FromCurrency && r.ToCurrency == rate.ToCurrency);

            //if (existing != null)
            //{
            //    existing.Rate = rate.Rate;
            //    existing.UpdatedAt = DateTime.UtcNow;
            //}
            //else
            //{
                rate.UpdatedAt = DateTime.UtcNow;
                _context.ExchangeRates.Add(rate);
            //}

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("param USD")]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRatesByParam(decimal param, bool isAbove)
        {   
            var exRates = await _context.ExchangeRates.ToListAsync();
            if(isAbove){
            exRates = await _context.ExchangeRates
            .Where(c => c.FromCurrency=="USD" && c.Rate >= param)
            .ToListAsync();
            }
            else{
            exRates= await _context.ExchangeRates
            .Where(c => c.Rate <= param)
            .ToListAsync();
            }
            return exRates;
        }

        [HttpGet("date param")]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRatesByDateRange(DateTime sDate, DateTime eDate)
        {   
            var exRates = await _context.ExchangeRates.ToListAsync();
            
            exRates = await _context.ExchangeRates
            .Where(c => c.UpdatedAt >= sDate && c.UpdatedAt <= eDate)
            .ToListAsync();
            
            
            return exRates;
        }

        [HttpGet("date param avg")]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRatesAvgByDateRange(DateTime sDate, DateTime eDate)
        {   
            var averages = await _context.ExchangeRates
        .Where(r => r.UpdatedAt >= sDate && r.UpdatedAt <= eDate)
        .GroupBy(r => new { r.FromCurrency, r.ToCurrency })
        .Select(g => new ExchangeRateAverageDto
        {
            FromCurrency = g.Key.FromCurrency,
            ToCurrency = g.Key.ToCurrency,
            AverageRate = Math.Round(g.Average(r => r.Rate),2)
        })
        .ToListAsync();
            
            return Ok(averages);
        }
        
          [HttpGet("calculate")]
        public async Task<IActionResult> Calculate(string input)
        {
            string[] tokens = input.Split(' ');

            decimal leftOperand = decimal.Parse(tokens[0]);
            string operatorSymbol = tokens[1];
            string currencyCode = tokens[2];
            var currency = await _context.ExchangeRates
            .Where(c => c.ToCurrency == currencyCode)
            .FirstOrDefaultAsync();
            decimal excRate = currency.Rate;
            decimal result=0;
            switch (operatorSymbol){
                case "/" :
                    result= leftOperand/excRate;
                    break;
                case "*" :
                    result= leftOperand*excRate;
                    break;
            }
            
            return Ok(Math.Round(result, 2).ToString("0.00"));
        } 
        
        
    }

    internal class ExchangeRateAverageDto
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal AverageRate { get; set; }
    }
}
