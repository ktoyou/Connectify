using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GachiHubBackend.Jwt;
using GachiHubBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GachiHubBackend.Controllers;

[Route("Api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly UserRepository _userRepository;
    
    private readonly JwtConfiguration _jwtConfiguration;
    
    public AuthorizationController(UserRepository repository, JwtConfiguration jwtConfiguration)
    {
        _userRepository = repository;
        _jwtConfiguration = jwtConfiguration;
    }

    [HttpGet(nameof(Login))]
    public async Task<IActionResult> Login(string login, string password)
    {
        var user = await _userRepository.GetUserByLoginAndPasswordAsync(login, password);
        if (user == null)
        {
            return BadRequest(new { message = "Login or password is incorrect" });
        }
        
        return Ok(new
        {
            token = GetToken(login)
        });
    }

    private string GetToken(string login)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, login),
        };

        var jwt = new JwtSecurityToken(issuer: _jwtConfiguration.Issuer, 
            audience: _jwtConfiguration.Audience, 
            claims: claims, 
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(
                _jwtConfiguration.GetSymmetricSecurityKey(), 
                SecurityAlgorithms.HmacSha256
            )
        );
        
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return token;
    }
}