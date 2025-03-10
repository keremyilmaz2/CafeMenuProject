
using AutoMapper;
using Calia.Services.ShoppingCartAPI;
using Calia.Services.ShoppingCartAPI.Data;
using Calia.Services.ShoppingCartAPI.Extensions;
using Calia.Services.ShoppingCartAPI.Utility;
using Calia.Services.ShoppingCartAPI.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Calia.Services.ShoppingCartAPI.Service.IService;
using Calia.Services.ShoppingCartAPI.Hubs;
using Calia.Services.ShoppingCartAPI.Controllers;
using Serilog;
using Calia.Services.ShoppingCartAPI.Middleware;
using Serilog.Formatting.Json;
using Serilog.Sinks.Grafana.Loki;
var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("Service", "ShoppingCartAPI") // Servis ismini ekle
        .Enrich.FromLogContext()
        .WriteTo.Console(new JsonFormatter()) // 🌟 Promtail için JSON formatında log
        .WriteTo.File(new JsonFormatter(), "/app/logs/shoppingCart-api.log", rollingInterval: RollingInterval.Day) // 📂 JSON formatında dosya logu
        .WriteTo.Seq("http://seq_log_service:5341") // 🚀 SEQ log servisine gönder
        .WriteTo.GrafanaLoki("http://loki:3100", labels: new List<LokiLabel>
        {
            new LokiLabel { Key = "app", Value = "shoppingCart-api" },
            new LokiLabel { Key = "env", Value = "docker" }
        }); // 📊 Loki'ye log gönder
});


var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackEndApiAuthenticationHttpClientHandler>();
builder.Services.AddHttpClient("Product", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackEndApiAuthenticationHttpClientHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
			},new string[] { }
		}
	});
});
builder.AddAppAuthetication();
builder.Services.AddAuthorization();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", builder =>
//    {
//        builder.WithOrigins("https://localhost:7270", "https://shoppingcart.justkey.online", "https://justkey.online") // İstemci uygulamanızın URL'si
//               .AllowAnyMethod()
//               .AllowAnyHeader()
//               .AllowCredentials(); // Kimlik bilgilerini kullanmayı sağlamak için
//    });
//});


builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true) // Tüm originleri kabul et
    );
});

// SignalR servisini ekle
builder.Services.AddSignalR();

var app = builder.Build();
app.UseMiddleware<LoggingMiddleware>();

// HTTP request pipeline'ı yapılandırma
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseRouting();
//app.UseCors("AllowSpecificOrigin"); 
app.UseCors("SignalRCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CartHub>("/hubs/CartHub");

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
