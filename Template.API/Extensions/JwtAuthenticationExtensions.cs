using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Template.Shared.Models.Configuration;

namespace Template.API.Extensions;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null)
        {
            throw new InvalidOperationException("JwtSettings configuration is missing");
        }

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/problem+json";
                        return context.Response.WriteAsJsonAsync(new
                        {
                            type = "https://api.template.local/errors/TOKEN_EXPIRED",
                            title = "Token Expired",
                            status = 401,
                            detail = "The access token has expired. Please refresh your token.",
                            traceId = context.Request.HttpContext.TraceIdentifier,
                            timestamp = DateTime.UtcNow
                        });
                    }

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/problem+json";
                    return context.Response.WriteAsJsonAsync(new
                    {
                        type = "https://api.template.local/errors/INVALID_TOKEN",
                        title = "Invalid Token",
                        status = 401,
                        detail = "The provided token is invalid.",
                        traceId = context.Request.HttpContext.TraceIdentifier,
                        timestamp = DateTime.UtcNow
                    });
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/problem+json";
                    return context.Response.WriteAsJsonAsync(new
                    {
                        type = "https://api.template.local/errors/UNAUTHORIZED",
                        title = "Unauthorized",
                        status = 401,
                        detail = "Authorization header is missing or invalid.",
                        traceId = context.Request.HttpContext.TraceIdentifier,
                        timestamp = DateTime.UtcNow
                    });
                }
            };
        });

        return services;
    }
}
