using HotelListing.API.Configurations;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Repository;
using HotelListing.API.DataLayer;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Db Context
var CONNECTION_STRING = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

builder.Services.AddDbContext<HotelListingDbContext>(DbOptions =>
{
    DbOptions.UseSqlServer(CONNECTION_STRING);
});

// Repositories

/* Generic Interface and Repository */
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // uses these interfaces/classes

/* Countries Interface and Repository */
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>(); // allows us to implement additional methods

/* Hotels Interface and Repository */
builder.Services.AddScoped<IHotelsRepository, HotelsRepository>();

// Add services to the container.
builder.Services.AddControllers();

// Adds AutoMapper Config
builder.Services.AddAutoMapper(typeof(MapperConfig));

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger Documentation/Debugging
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
