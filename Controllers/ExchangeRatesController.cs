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

namespace InvestmentPortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExchangeRatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExchangeRates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRates()
        {
            return await _context.ExchangeRates.ToListAsync();
        }

        // POST: api/ExchangeRates (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrUpdateRate(ExchangeRate rate)
        {
            var existing = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => r.FromCurrency == rate.FromCurrency && r.ToCurrency == rate.ToCurrency);

            if (existing != null)
            {
                existing.Rate = rate.Rate;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                rate.UpdatedAt = DateTime.UtcNow;
                _context.ExchangeRates.Add(rate);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        //Delete all
        [HttpDelete("delete-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAllRates()
        {
            var allRates = await _context.ExchangeRates.ToListAsync();
            _context.ExchangeRates.RemoveRange(allRates);
            await _context.SaveChangesAsync();

            return Ok("All exchange rates deleted.");
        }
        // GET: api/ExchangeRates/convert?from=USD&to=TRY&amount=100
        [HttpGet("convert")]
        public async Task<ActionResult<decimal>> ConvertCurrency(string from, string to, decimal amount)
        {
            if (from == to)
                return Ok(amount);

            var rate = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => r.FromCurrency == from && r.ToCurrency == to);

            if (rate == null)
                return NotFound("Exchange rate not found.");

            var result = amount * rate.Rate;
            return Ok(Math.Round(result, 2));
            
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("update-live-rates")]
        public async Task<IActionResult> UpdateLiveRates([FromServices] ExchangeRateUpdater updater)
        {
            var baseCurrencies = new[] { "USD", "EUR", "TRY", "GBP" };

            foreach (var baseCurrency in baseCurrencies)
            {
                await updater.UpdateRatesAsync(baseCurrency);
            }

            return Ok("All exchange rates updated from internet.");
        }
    }
}