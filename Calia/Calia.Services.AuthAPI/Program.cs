
using Calia.Services.AuthAPI.Data;
using Calia.Services.AuthAPI.Models;

using Calia.Services.AuthAPI.Service;
using Calia.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
// Add services to the container.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://auth.justkey.online", "https://auth.justkey.online") // İstemci uygulamanızın URL'si
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Kimlik bilgilerini kullanmayı sağlamak için
    });
});

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
	app.UseSwagger();
	app.UseSwaggerUI();
//}

app.UseHttpsRedirection();


app.UseRouting();
app.UseCors("AllowSpecificOrigin"); // CORS kullanımı


app.UseAuthentication();
app.UseAuthorization();

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