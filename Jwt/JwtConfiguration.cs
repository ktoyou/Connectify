using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GachiHubBackend.Jwt;

public class JwtConfiguration
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public SymmetricSecurityKey GetSymmetricSecurityKey()
        => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
}