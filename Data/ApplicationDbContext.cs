using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using InvestmentPortfolioAPI.Models;

namespace InvestmentPortfolioAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<InvestmentPortfolio> InvestmentPortfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InvestmentPortfolio>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2); // 18 digits total, 2 after decimal point

            modelBuilder.Entity<InvestmentPortfolio>()
                .Property(p => p.CostBasis)
                .HasPrecision(18, 2);    
                
            modelBuilder.Entity<ExchangeRate>()
                .Property(r => r.Rate)
                .HasPrecision(18, 6); // good for exchange rates like 0.938472
        }

        public DbSet<User> Users { get; set; }

        public DbSet<AssetType> AssetTypes { get; set; }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }
    }
}