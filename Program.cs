using HotelListing.API.Configurations;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataAccessLayer.Repository;
using HotelListing.API.DataAccessLayer.Services;
using HotelListing.API.DataLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Db Context
var CONNECTION_STRING = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

builder.Services.AddDbContext<HotelListingDbContext>(DbOptions =>
{
    DbOptions.UseSqlServer(CONNECTION_STRING);
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        adding => adding.AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod());
});

// Add Serilog
builder.Host.UseSerilog((builderContext, loggerConfig) => 
    loggerConfig.WriteTo.Console()
    .ReadFrom
    .Configuration(builderContext.Configuration));

/* 
 * 
 * Repositories
 * 
 */

/* Adds AutoMapper Config */
builder.Services.AddAutoMapper(typeof(MapperConfig));

/* Adds Identity Core */
builder.Services.AddIdentityCore<APIUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HotelListingDbContext>();

/* Generic Interface and Repository */
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // uses these interfaces/classes

/* Countries Interface and Repository */
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>(); // allows us to implement additional methods

/* Hotels Interface and Repository */
builder.Services.AddScoped<IHotelsRepository, HotelsRepository>();

/* IAuthManager */
builder.Services.AddScoped<IAuthManager, AuthManager>();

/* Authentication plus JWT Bearer */
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Audience"]))
        };
    }
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger Documentation/Debugging
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
