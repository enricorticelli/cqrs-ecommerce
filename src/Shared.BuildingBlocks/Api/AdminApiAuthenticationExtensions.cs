using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.BuildingBlocks.Api;

public static class AdminApiAuthenticationExtensions
{
    public static WebApplicationBuilder AddAdminApiAuthentication(this WebApplicationBuilder builder)
    {
        var signingKey = builder.Configuration["Account:Jwt:SigningKey"] ?? "dev-only-signing-key-change-me";
        var adminIssuer = builder.Configuration["Account:Jwt:AdminIssuer"] ?? "account-admin";
        var adminAudience = builder.Configuration["Account:Jwt:AdminAudience"] ?? "backoffice";

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateIssuer = true,
                    ValidIssuer = adminIssuer,
                    ValidateAudience = true,
                    ValidAudience = adminAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim("realm", "admin"));
        });

        return builder;
    }
}
