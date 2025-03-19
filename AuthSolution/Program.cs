
using System.Security.Claims;
using System.Text;
using AuthSolution.Extensions;
using AuthSolution.Model;
using AuthSolution.Ports;
using AuthSolution.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthSolution
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IAuth, AuthService>();

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["AuthSecretKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseAuthentication();

            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/JwtGenerator", async ([FromHeader] string nome, [FromServices] IAuth authService) =>
            {
                var token = authService.JwtAuthHandler(nome);
                return Results.Ok(token);
            });

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

            app.MapGet("/restrito", (ClaimsPrincipal userClaim) => new User
            {
                Id = userClaim.GetId(),
                Name = userClaim.GetName(),
                Email = userClaim.GetEmail(),
                Password = userClaim.GetPassword(),
                Role = userClaim.GetRoles()
            }).RequireAuthorization().WithDescription("Retorna os Claims contidos dentro do JWT.");

            app.MapGet("/admin", () => "TESTE ADM").RequireAuthorization("admin").WithDescription("Testa a role do usuário atrelado ao Claim do JWT.");

            app.Run();
        }
    }
}
