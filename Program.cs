using InvestmentPortfolioAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InvestmentPortfolioAPI.Models;
using InvestmentPortfolioAPI.Models.Mongo;
using InvestmentPortfolioAPI.Services;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(); // required for HttpClient
builder.Services.AddScoped<ExchangeRateUpdater>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{   
    options.SupportNonNullableReferenceTypes();
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("UserRateLimit", context =>
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == true && user.IsInRole("User"))
        {
            return RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: user.Identity.Name ?? "anonymous",
                factory: key => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    TokensPerPeriod = 10,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    AutoReplenishment = true
                });
        }

        // No limit for Admins
        return RateLimitPartition.GetNoLimiter(user.Identity?.Name ?? "anonymous");
    });
});
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<MongoLoggerService>();
builder.Services.AddHttpClient<ExchangeRateUpdater>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };

            
     /* Used for debugging reasons
       options.Events = new JwtBearerEvents
{
    OnAuthenticationFailed = context =>
    {
        Console.WriteLine("JWT auth failed: " + context.Exception.Message);
        return Task.CompletedTask;
    },
    OnTokenValidated = context =>
    {
        Console.WriteLine("JWT token validated successfully.");
        return Task.CompletedTask;
    }
};*/

    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "amail",
                Role = "Admin"
            },
            new User
            {
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Email = "umail",
                Role = "User"
            }
        );
        db.SaveChanges();
    }
    
     if (!db.AssetTypes.Any())
    {
        db.AssetTypes.AddRange(
            new AssetType { Name = "Cash" },
            new AssetType { Name = "Stocks" },
            new AssetType { Name = "Cryptocurrency" },
            new AssetType { Name = "Gold" }
        );
        db.SaveChanges();
    }
    if (!db.Currencies.Any())
        {
            db.Currencies.AddRange(
                new Currency { Name = "TRY" , Symbol="₺" },
                new Currency { Name = "USD" , Symbol="$" },
                new Currency { Name = "JPY" , Symbol="¥" },
                new Currency { Name = "EUR" , Symbol="€" }
            );
            db.SaveChanges();
        }
   

}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();