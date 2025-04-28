using GachiHubBackend.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GachiHubBackend.Extensions;

public static class AuthenticationBuilderExtensions
{
    public static void RegisterAuthentication(this AuthenticationBuilder builder, JwtConfiguration jwt)
    {
        builder.AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                
                ValidateLifetime = true,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwt.GetSymmetricSecurityKey(),
            };

            opts.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/connectify"))
                    {
                        context.Token = accessToken;
                    }
                    
                    return Task.CompletedTask;
                }
            };
        });
    }
}