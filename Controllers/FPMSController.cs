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
        
        /*[HttpGet("calculate")]
        public async Task<IActionResult> Calculate(string input)
        {
            var formula = "price * tax + 5";

            var parameters = await _context.ExchangeRates.ToListAsync();
            var expression = input;

            foreach (var param in parameters)
            {
                expression.Parameters[param.ToCurrency] = param.Rate;
            }

            try
            {
                var result = expression.Evaluate();
                return Ok(new { formula, result });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error in formula: {ex.Message}");
            } */
        }
    }
