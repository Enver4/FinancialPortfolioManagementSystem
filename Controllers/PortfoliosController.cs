using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentPortfolioAPI.Data;
using InvestmentPortfolioAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InvestmentPortfolioAPI.Models.Dto;
using InvestmentPortfolioAPI.Services;
using InvestmentPortfolioAPI.Models.Mongo;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace InvestmentPortfolioAPI.Controllers
{
    [Authorize] //Protects all endpoints in this controller
    [ApiController]
    [Route("api/[controller]")]
    
    
    public class PortfoliosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MongoLoggerService _mongoLogger;

        public PortfoliosController(
            ApplicationDbContext context,
            MongoLoggerService mongoLogger
        )
        {
            _context = context;
            _mongoLogger = mongoLogger;
        }

        // GET: api/Portfolios
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InvestmentPortfolio>>> GetPortfolios()
        {
            return await _context.InvestmentPortfolios.ToListAsync();
        }

        
        
        // GET: api/Portfolios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvestmentPortfolio>> GetPortfolio(int id)
        {
            var portfolio = await _context.InvestmentPortfolios.FindAsync(id);
            if (portfolio == null)
                return NotFound();

            return portfolio;
        }

        
        // POST: api/Portfolios
        [HttpPost]
        public async Task<ActionResult<InvestmentPortfolio>> PostPortfolio(InvestmentPortfolio portfolio)
        {
            var userId = GetUserId();

            // Optional: Validate asset type exists
            var assetTypeExists = await _context.AssetTypes.AnyAsync(a => a.Id == portfolio.AssetTypeId);
            if (!assetTypeExists)
                return BadRequest("Invalid AssetTypeId");

            portfolio.UserId = userId;

            _context.InvestmentPortfolios.Add(portfolio);
            await _context.SaveChangesAsync();
            await _mongoLogger.LogAsync(new LogEntry
            {
                UserId = GetUserId().ToString(),
                Type = "Transaction",
                Message = $"Created portfolio: {portfolio.Name}",
                Data = portfolio
            });
            return CreatedAtAction(nameof(GetPortfolio), new { id = portfolio.Id }, portfolio);
        }

        // PUT: api/Portfolios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio(int id, InvestmentPortfolio portfolio)
        {
            if (id != portfolio.Id)
                return BadRequest();

            _context.Entry(portfolio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.InvestmentPortfolios.Any(p => p.Id == id))
                    return NotFound();
                throw;
            }
            await _mongoLogger.LogAsync(new LogEntry
            {
                UserId = GetUserId().ToString(),
                Type = "Transaction",
                Message = $"Updated portfolio: {portfolio.Name}",
                Data = portfolio
            });

            return NoContent();
        }

        // DELETE
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(int id)
        {
            var portfolio = await _context.InvestmentPortfolios.FindAsync(id);
            if (portfolio == null)
                return NotFound();

            _context.InvestmentPortfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
            await _mongoLogger.LogAsync(new LogEntry
            {
                UserId = GetUserId().ToString(),
                Type = "Transaction",
                Message = $"Deleted portfolio: {portfolio.Name}",
                Data = portfolio
            });
            return NoContent();
        }
        private int GetUserId()
        {
        var username = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        return user?.Id ?? 0;
        }
        
        [HttpGet("evaluate")]
        public async Task<ActionResult<decimal>> EvaluatePortfolio([FromQuery] string targetCurrency = "USD")
        {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var userId = GetUserId();

        // Get the user's portfolios (or all if admin)
        var portfolios = await _context.InvestmentPortfolios
            .Include(p => p.AssetType)
            .Where(p => role == "Admin" || p.UserId == userId)
            .ToListAsync();

        decimal total = 0;

        foreach (var p in portfolios)
        {
            if (p.Currency == targetCurrency)
            {
                total += p.Amount;
            }
            else
            {
                var rate = await _context.ExchangeRates
                    .FirstOrDefaultAsync(r => r.FromCurrency == p.Currency && r.ToCurrency == targetCurrency);

                if (rate != null)
                {
                    total += p.Amount * rate.Rate;
                }
                // Optionally: handle missing rate (skip or throw)
            }
        }

        return Ok(Math.Round(total, 2));
        }
        

        //REPORT

        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<PortfolioReportDto>>> GetProfitLossReport([FromQuery] string targetCurrency = "USD")
        {
        var userId = GetUserId();
        var role = User.FindFirstValue(ClaimTypes.Role);

        var portfolios = await _context.InvestmentPortfolios
            .Include(p => p.AssetType)
            .Where(p => role == "Admin" || p.UserId == userId)
            .ToListAsync();

        var report = new List<PortfolioReportDto>();

        foreach (var p in portfolios)
        {
            decimal convertedAmount = p.Amount;

            if (p.Currency != targetCurrency)
            {
                var rate = await _context.ExchangeRates
                    .FirstOrDefaultAsync(r => r.FromCurrency == p.Currency && r.ToCurrency == targetCurrency);

                if (rate != null)
                {
                    convertedAmount = p.Amount * rate.Rate;
                }
                else
                {
                    // Skip if no rate is found
                    continue;
                }
            }

            decimal convertedCost = p.CostBasis;

        if (p.Currency != targetCurrency)
        {
            var costRate = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => r.FromCurrency == p.Currency && r.ToCurrency == targetCurrency);

            if (costRate != null)
            {
                convertedCost = p.CostBasis * costRate.Rate;
            }
            else
            {
                continue; // Skip if no rate
            }
        }

        report.Add(new PortfolioReportDto
        {
            Name = p.Name,
            Currency = p.Currency,
            Amount = p.Amount,
            CostBasis = Math.Round(convertedCost, 2),
            CurrentValue = Math.Round(convertedAmount, 2)
        });
        }

        return Ok(report);
        }
        
       /* [HttpPost("upload")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> UploadCsv([FromForm] FileUploadDto dto)
{
    var file = dto.File;

    if (file == null || file.Length == 0)
        return BadRequest("No file uploaded.");

    return Ok($"Received file: {file.FileName}");
} */

        /* [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv([FromForm] FileUploadDto dto)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            int lineNumber = 0;
            var lines = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                lineNumber++;

                if (lineNumber == 1) continue; // skip header
                lines.Add(line ?? "");
            }

            return Ok(new
            {
                Message = "File read successfully.",
                LineCount = lines.Count
            });
        } */

        //UPLOADING CSV

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv([FromForm] FileUploadDto dto)
        {
            var file = dto.File;
            var userId = GetUserId();

            var result = new
            {
                Successes = new List<string>(),
                Errors = new List<string>()
            };

            if (file == null || file.Length == 0)
            {
                result.Errors.Add("No file uploaded.");
                return BadRequest(result);
            }

            using var reader = new StreamReader(file.OpenReadStream());
            int lineNumber = 0;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                lineNumber++;

                if (lineNumber == 1) continue; // skip header

                var parts = line?.Split(',');

                if (parts == null || parts.Length != 5)
                {
                    result.Errors.Add($"Line {lineNumber}: Invalid format (expected 5 columns).");
                    continue;
                }

                string name = parts[0].Trim();
                bool amountOk = decimal.TryParse(parts[1], out decimal amount);
                string currency = parts[2].Trim();
                bool costOk = decimal.TryParse(parts[3], out decimal costBasis);
                bool assetTypeOk = int.TryParse(parts[4], out int assetTypeId);

                if (!amountOk || !costOk || !assetTypeOk || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(currency))
                {
                    result.Errors.Add($"Line {lineNumber}: Invalid data values.");
                    continue;
                }

                var portfolio = new InvestmentPortfolio
                {
                    Name = name,
                    Amount = amount,
                    Currency = currency,
                    CostBasis = costBasis,
                    AssetTypeId = assetTypeId,
                    UserId = userId
                };

                _context.InvestmentPortfolios.Add(portfolio);
                result.Successes.Add($"Line {lineNumber}: Added '{name}'");
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("🔥 Upload completed — about to log to MongoDB");
            await _mongoLogger.LogAsync(new LogEntry
            {
                UserId = GetUserId().ToString(),
                Type = "Upload",
                Message = "CSV upload processed",
                Data = new
                {
                    FileName = dto.File.FileName,
                    SuccessCount = result.Successes.Count,
                    ErrorCount = result.Errors.Count,
                    Errors = result.Errors
                }
            });
            return Ok(result);
        }
    }
}