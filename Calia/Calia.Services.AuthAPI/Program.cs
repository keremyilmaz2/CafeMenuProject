using Calia.Services.AuthAPI.Data;
using Calia.Services.AuthAPI.Middleware;
using Calia.Services.AuthAPI.Models;
using Calia.Services.AuthAPI.Service;
using Calia.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırmasını güncelle (Servis ismini ekle)
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("Service", "AuthAPI") // Servis ismi ekleniyor
        .Enrich.FromLogContext();
});

var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();

// MSSQL Veritabanı Konfigürasyonu
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

// JWT ve Authentication Konfigürasyonu
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

// CORS Politikası
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

// 📌 **Serilog Middleware'i En Üste Aldım** (Tüm istekleri loglayabilmesi için)
app.UseMiddleware<LoggingMiddleware>();
// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS Zorunlu
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

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
        // Veritabanı migrasyonlarını uygula
        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
            Log.Information("✅ Veritabanı migrasyonları başarıyla uygulandı.");
        }
        else
        {
            Log.Information("🔍 Uygulanacak veritabanı migrasyonu bulunamadı.");
        }
    }
    catch (Exception ex)
    {
        // Hata oluşursa logla
        Log.Error(ex, "❌ Veritabanı migrasyonu sırasında hata oluştu.");
    }
}
