using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace InvestmentPortfolioAPI.Models.Dto
{
    public class FileUploadDto
    {
        public IFormFile File { get; set; } = null!;
    }
}