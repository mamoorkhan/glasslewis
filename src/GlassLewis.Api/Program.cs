using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using GlassLewis.Application.Services;
using GlassLewis.Domain.Interfaces;
using GlassLewis.Infrastructure.Data.Contexts;
using GlassLewis.Infrastructure.Mapping;
using GlassLewis.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace GlassLewis.Api;

/// <summary>
/// Represents the entry point of the application.
/// </summary>
/// <remarks>This method initializes the application by configuring services, middleware, and the HTTP request
/// pipeline. It sets up authentication, database context, API versioning, CORS policies, and Swagger/OpenAPI
/// documentation. The application is then started and begins listening for incoming HTTP requests.</remarks>
[ExcludeFromCodeCoverage]
public class Program
{
    /// <summary>
    /// The main method that starts the application.
    /// </summary>
    /// <param name="args">An array of command-line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

        // Add Entity Framework
        builder.Services.AddDbContext<CompanyDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add repositories
        builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

        // Add services
        builder.Services.AddScoped<ICompanyService, CompanyService>();

        builder.Services.AddCors(options =>
        {
            var environment = builder.Environment;

            if (environment.EnvironmentName is "dev" or "pipeline")
            {
                options.AddPolicy("DevelopmentCors", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            }

            options.AddPolicy("ProductionCors", policy =>
            {
                policy.WithOrigins("https://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        // Specify the exact method overload to resolve ambiguity
        builder.Services.AddAutoMapper(typeof(IProfile).Assembly);

        builder.Services.AddControllers();

        // Configure API versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("ver"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = """
                              JWT Authorization header using the Bearer scheme. \r\n\r\n
                                                    Enter 'Bearer' [space] and then your token in the text input below.
                                                    \r\n\r\nExample: 'Bearer 12345abcdef'
                              """,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
