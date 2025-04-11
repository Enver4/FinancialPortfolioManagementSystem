using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentPortfolioAPI.Data;
using InvestmentPortfolioAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace InvestmentPortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssetTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AssetTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetType>>> GetAssetTypes()
        {
            return await _context.AssetTypes.ToListAsync();
        }

        // POST: api/AssetTypes
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<AssetType>> AddAssetType(AssetType type)
        {
            _context.AssetTypes.Add(type);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAssetTypes), new { id = type.Id }, type);
        }
    }
}