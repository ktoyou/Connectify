using GachiHubBackend.Jwt;

namespace GachiHubBackend.Extensions;

public static class JwtConfigurationExtension
{
    public static JwtConfiguration GetJwtConfiguration(this WebApplicationBuilder builder)
    {
        var audience = builder.Configuration["Jwt:Audience"];
        var issuer = builder.Configuration["Jwt:Issuer"];
        var key = builder.Configuration["Jwt:Key"];

        ArgumentNullException.ThrowIfNull(audience);
        ArgumentNullException.ThrowIfNull(issuer);
        ArgumentNullException.ThrowIfNull(key);

        var jwtConfiguration = new JwtConfiguration()
        {
            Issuer = issuer,
            Audience = audience,
            Key = key
        };
        
        builder.Services.AddSingleton(jwtConfiguration);

        return jwtConfiguration;
    }
}