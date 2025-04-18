using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentPortfolioAPI.Data;
using InvestmentPortfolioAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace InvestmentPortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CurrenciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Currencies
        [HttpGet]
        [EnableRateLimiting("UserRateLimit")]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            return await _context.Currencies.ToListAsync();
        }

        // POST: api/Currencies
        
        [HttpPost]
        public async Task<ActionResult<Currency>> AddCurrency(Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCurrencies), new { id = currency.Id }, currency);
        }

        [HttpGet("{symbol}")]
        public async Task<ActionResult<Currency>> GetCurrencyBySymbol(String symbol)
        {
            var currency = await _context.Currencies.SingleOrDefaultAsync(s=> s.Symbol == symbol);
            if (currency == null)
                return NotFound();

            return currency;
        }

            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
                return NotFound();

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

    }
}