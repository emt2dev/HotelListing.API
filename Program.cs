using HotelListing.API.Configurations;
using HotelListing.API.DataAccessLayer.Interfaces;
using HotelListing.API.DataAccessLayer.Models;
using HotelListing.API.DataAccessLayer.Repository;
using HotelListing.API.DataAccessLayer.Services;
using HotelListing.API.DataLayer;
using HotelListing.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Db Context
var CONNECTION_STRING = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

builder.Services.AddDbContext<HotelListingDbContext>(DbOptions =>
{
    DbOptions.UseSqlServer(CONNECTION_STRING);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Listing API",
        Version = "v1",
    });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = @"Jwt Authorization header using bearer scheme.
                        Enter 'Bearer' [space] and then token.
                        Example: 'Bearer 12345'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "0auth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

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
    .AddTokenProvider<DataProtectorTokenProvider<APIUser>>("HotelListingAPI")
    .AddEntityFrameworkStores<HotelListingDbContext>()
    .AddDefaultTokenProviders();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        };
    }
);

/* API Versioning */
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Version"),
            new MediaTypeApiVersionReader("ver")
        );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

/* Add Caching Part One */
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;
});

// Add services to the container.
builder.Services.AddControllers()
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy();
    });

/* Health Checks, 
 * includes AspNetCore.HealthChecks.SqlServer
 * check to see if EFCore<DbContext> can connect via Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
 */

builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("Custom Health Check",
    failureStatus: HealthStatus.Degraded,
    tags: new[] { "Custom" }
    )
    .AddSqlServer(CONNECTION_STRING, tags: new[] { "database" })
    .AddDbContextCheck<HotelListingDbContext>(tags: new[] { "database" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger Documentation/Debugging
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

/* Add Caching Part Two */
app.UseResponseCaching();

app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
    {
        Public = true,
        MaxAge = TimeSpan.FromSeconds(10) // refresh the cache every this many seconds
    };

    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };

    await next();
});

/* Health Checks */
/* Health Checks */
app.MapHealthChecks("/healthcheck");

app.MapHealthChecks("/healthcheckCustom", new HealthCheckOptions
{
    Predicate = HealthCheck => HealthCheck.Tags.Contains("custom"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
    ResponseWriter = WriteResponse
});

/* Health Check for DB */
app.MapHealthChecks("/healthcheckdatabase", new HealthCheckOptions
{
    Predicate = HealthCheck => HealthCheck.Tags.Contains("database"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
    ResponseWriter = WriteResponse
});

/* Health Check Log */
static Task WriteResponse(HttpContext context, HealthReport HR)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonWriterOptions { Indented = true };

    using var memoryStream = new MemoryStream();
    using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString("status", HR.Status.ToString());
        jsonWriter.WriteStartObject("results");

        foreach (var entry in HR.Entries)
        {
            jsonWriter.WriteStartObject(entry.Key);
            jsonWriter.WriteString("status", entry.Value.Status.ToString());

            if (entry.Value.Description != null) jsonWriter.WriteString("description", entry.Value.Description.ToString());

            jsonWriter.WriteStartObject("data");

            foreach (var item in entry.Value.Data)
            {
                jsonWriter.WritePropertyName(item.Key);
                JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();
    }

    return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
