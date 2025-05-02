using Connectify.Db.Model;
using FluentValidation;
using GachiHubBackend.Attributes;
using GachiHubBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GachiHubBackend.Controllers;

[Route("Api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _userRepository;
    
    private readonly IValidator<User> _userValidator;
    
    public UsersController(UserRepository usersRepository, IValidator<User> userValidator)
    {
        _userRepository = usersRepository;
        _userValidator = userValidator;
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

        var newUser = new User()
        {
            Login = login,
            Password = password,
            CreatedAt = DateTime.Now
        };
        
        var validationResult = await _userValidator.ValidateAsync(newUser);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList()
            });
        }
        
        
        await _userRepository.AddAsync(newUser);

        return Ok(new
        {
            message = "User created"
        });
    }
    
    [HttpGet(nameof(GetProfile))]
    [Authorize]
    [CurrentUser]
    public Task<IActionResult> GetProfile()
    {
        var user = HttpContext.Items["CurrentUser"] as User;
        return Task.FromResult<IActionResult>(Ok(new
        {
            user
        }));
    }
}