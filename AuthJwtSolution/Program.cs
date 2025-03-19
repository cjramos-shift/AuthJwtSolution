using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using AuthJwtSolution.Ports;
using AuthJwtSolution.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using AuthJwtSolution.Extensions;
using AuthJwtSolution.Model;
using System.Text.Json;
using AuthJwtSolution.Context;

namespace AuthJwtSolution;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.TypeInfoResolver = UserContext.Default;
        });


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

        builder.Services.AddAuthorization(x =>
        {
            x.AddPolicy("Admin", p => p.RequireRole("admin"));
        });

        var app = builder.Build();

        app.UseAuthentication();

        app.UseAuthorization();

        var jwtAuth = app.MapGet("/JwtGenerator", async ([FromHeader] string nome, [FromServices] IAuth authService) =>
        {
            var token = authService.JwtAuthHandler(nome);
            return Results.Ok(token);
        });

        var sampleTodos = new Todo[] {
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
        };

        var todosApi = app.MapGroup("/todos");
        todosApi.MapGet("/", () => sampleTodos).RequireAuthorization();
        todosApi.MapGet("/{id}", (int id) =>
            sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
                ? Results.Ok(todo)
                : Results.NotFound());

        var restrito = app.MapGet("/restrito", (ClaimsPrincipal userClaim) => new User
        {
            Id = userClaim.GetId(),
            Name = userClaim.GetName(),
            Email = userClaim.GetEmail(),
            Password = userClaim.GetPassword(),
            Role = userClaim.GetRoles()
        }).RequireAuthorization().WithDescription("Retorna os Claims contidos dentro do JWT.");

        var adminApi = app.MapGroup("/admin").RequireAuthorization("admin").WithDescription("Testa a role do usuário atrelado ao Claim do JWT."); ;

        app.Run();
    }
}

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
