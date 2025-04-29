using Connectify.Db.Model;
using GachiHubBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GachiHubBackend.Controllers;

[Route("Api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserRepository _userRepository;
    
    public UsersController(UserRepository usersRepository)
    {
        _userRepository = usersRepository;
    }

    [HttpGet(nameof(Register))]
    public async Task<IActionResult> Register(string login, string password)
    {
        var user = await _userRepository.GetUserByLoginAsync(login);
        if (user != null)
        {
            return BadRequest(new
            {
                message = "User already exists"
            });
        }

        await _userRepository.AddAsync(new User()
        {
            Login = login,
            Password = password,
            CreatedAt = DateTime.Now
        });

        return Ok(new
        {
            message = "User created"
        });
    }
    
    [HttpGet(nameof(GetProfile))]
    public async Task<IActionResult> GetProfile()
    {
        if (User.Identity == null || User.Identity?.Name == null)
        {
            return BadRequest(new
            {
                message = "Identity claim is missing or name is required"
            });
        }
        
        var user = await _userRepository.GetUserByLoginAsync(User.Identity.Name);

        return Ok(new
        {
            user
        });
    }
}