using AutoMapper;
using Calia.Services.StockAPI;
using Calia.Services.StockAPI.Data;
using Calia.Services.StockAPI.Extensions;
using Calia.Services.StockAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Grafana.Loki;
using System;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("Service", "StockAPI") // Servis ismini ekle
        .Enrich.FromLogContext()
        .WriteTo.Console(new JsonFormatter()) // 🌟 Promtail için JSON formatında log
        .WriteTo.File(new JsonFormatter(), "/app/logs/stock-api.log", rollingInterval: RollingInterval.Day) // 📂 JSON formatında dosya logu
        .WriteTo.Seq("http://seq_log_service:5341") // 🚀 SEQ log servisine gönder
        .WriteTo.GrafanaLoki("http://loki:3100", labels: new List<LokiLabel>
        {
            new LokiLabel { Key = "app", Value = "stock-api" },
            new LokiLabel { Key = "env", Value = "docker" }
        }); // 📊 Loki'ye log gönder
});


var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();
// SQL Server bağlanma
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

// AutoMapper ayarları
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Kontrolcüler
builder.Services.AddControllers();

// Swagger ayarları
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: 'Bearer Generated-JWT-Token'",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[] { }
        }
    });
});

// JWT Authentication
builder.AddAppAuthetication();
builder.Services.AddAuthorization();

// CORS ayarları
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://stock.justkey.online", "https://stock.justkey.online")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// HttpClient ayarları (SSL doğrulamasını devre dışı bırakma)
var httpClientHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Tüm sertifikaları kabul et
};

builder.Services.AddHttpClient("StockAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:StockAPI"]);
}).ConfigurePrimaryHttpMessageHandler(() => httpClientHandler); // HttpClient'a handler'ı ekle

var app = builder.Build();
app.UseMiddleware<LoggingMiddleware>();

// Swagger kullanımı
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }


}