using Calia.Services.AuthAPI.Data;
using Calia.Services.AuthAPI.Models;
using Calia.Services.AuthAPI.Service;
using Calia.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Serilog Konfigürasyonu
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/auth-api-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // Eski logları temizler
    .CreateLogger();

builder.Host.UseSerilog();

// Ortam değişkenlerini yükle
var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();

// MSSQL Veritabanı Konfigürasyonu
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

// Identity Servisi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT ve Authentication Servisleri
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Yetkilendirme
builder.Services.AddAuthorization();

// CORS Politikası (Önerilen)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://auth.justkey.online", "https://auth.justkey.online")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Serilog Middleware
app.UseSerilogRequestLogging();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS Zorunlu
app.UseHttpsRedirection();

// CORS Kullanımı
app.UseRouting();
app.UseCors("AllowSpecificOrigin");

// Authentication ve Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Controller Mapping
app.MapControllers();

// Veritabanı Migrasyonları
ApplyMigration();

app.Run();

void ApplyMigration()
{
    using var scope = app.Services.CreateScope();
    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
            Log.Information("Veritabanı migrasyonları başarıyla uygulandı.");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Veritabanı migrasyonu sırasında hata oluştu.");
    }
}
