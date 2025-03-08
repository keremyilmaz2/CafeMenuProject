using Calia.Web.Service;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Calia.Services.ShoppingCartAPI.RabbitMQSender;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using Calia.Web.Messaging;
using Mango.Services.EmailAPI.Extension;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// HTTP Client Registrations
builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddHttpClient<ICategoryService, CategoryService>();
builder.Services.AddHttpClient<IStockService, StockService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();

// Service URLs configuration
SD.StockAPIBase = builder.Configuration["ServiceUrls:StockAPI"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.CategoryAPIBase = builder.Configuration["ServiceUrls:CategoryAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
SD.OrderAPIAPIBase = builder.Configuration["ServiceUrls:OrderAPI"];

// Dependency Injection

builder.Services.AddScoped<IMessageBus, MessageBus>();


builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
// Hosted service registration

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(300);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddResponseCaching(); // Response caching service

builder.Services.AddControllers(option =>
{
    option.CacheProfiles.Add("Default300",
       new CacheProfile()
       {
           Duration = 14400 
       });
    option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
}).AddXmlDataContractSerializerFormatters();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://justkey.online", "https://justkey.online", "http://www.justkey.online", "https://www.justkey.online")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("tr-TR"),
        new CultureInfo("en-US"),
    };

    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseResponseCaching();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/login", context =>
    {
        context.Response.Redirect("/Auth/Login");
        return Task.CompletedTask;
    });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=MainIndex}/{id?}");
app.UseAzureServiceBusConsumer();
app.Run();
