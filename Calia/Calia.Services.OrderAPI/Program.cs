using Calia.Services.OrderAPI.Data;
using AutoMapper;
using Calia.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Calia.Services.OrderAPI;
using Calia.Services.OrderAPI.Service;
using Calia.Services.OrderAPI.Extensions;
using Calia.Services.OrderAPI.Service.IService;
using Calia.Services.OrderAPI.Services;
using Microsoft.AspNetCore.SignalR;
using Calia.Services.OrderAPI.Hubs;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();
// DbContext ayarları
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

// AutoMapper ve diğer servislere ekleme
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IProductService, Calia.Services.OrderAPI.Service.ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackEndApiAuthenticationHttpClientHandler>();

// HTTP Client ayarları
builder.Services.AddHttpClient("Product", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackEndApiAuthenticationHttpClientHandler>();
builder.Services.AddHttpClient("Stock", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:StockAPI"]))
    .AddHttpMessageHandler<BackEndApiAuthenticationHttpClientHandler>();
builder.Services.AddHttpClient("Auth", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:AuthAPI"]))
    .AddHttpMessageHandler<BackEndApiAuthenticationHttpClientHandler>();

builder.Services.AddHttpClient<IPrintNodeService, PrintNodeService>(client =>
{
    client.BaseAddress = new Uri("https://api.printnode.com/");
});

// PrintNode ve PrinterSettings ayarları
builder.Services.Configure<PrintNodeSettings>(builder.Configuration.GetSection("PrintNode"));
builder.Services.Configure<PrinterSettings>(builder.Configuration.GetSection("PrinterSettings"));

// Controller ayarları
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: 'Bearer Generated-JWT-Token'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.AddAppAuthetication();
builder.Services.AddAuthorization();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", builder =>
//    {
//        builder.WithOrigins("https://api.printnode.com/", "https://justkey.online", "https://localhost:7270", "https://order.justkey.online") // İstemci uygulamanızın URL'si
//               .AllowAnyMethod()
//               .AllowAnyHeader()
//               .AllowCredentials(); // Kimlik bilgilerini kullanmayı sağlamak için
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Kimlik bilgilerini kullanmayı sağlamak için
    });
});
// SignalR servisini ekle
builder.Services.AddSignalR();

var app = builder.Build();

//// HTTP request pipeline'ı yapılandırma
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseHttpsRedirection();

app.UseRouting();
//app.UseCors("AllowSpecificOrigin"); // CORS kullanımı
app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<TableHub>("/hubs/TableHub");

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