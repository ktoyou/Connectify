using System.Security.Claims;
using GachiHubBackend.Repositories;
using GachiHubBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GachiHubBackend.Controllers;

[Route("/Api/[controller]")]
[Authorize]
public class AvatarController : ControllerBase
{
    private readonly UserRepository _userRepository;
    
    private readonly AvatarService _avatarService;
    
    public AvatarController(UserRepository userRepository, AvatarService avatarService)
    {
        _userRepository = userRepository;
        _avatarService = avatarService;
    }

    [HttpPost(nameof(SetAvatar))]
    public async Task<IActionResult> SetAvatar(IFormFile? avatar)
    {
        if (avatar == null || avatar.Length == 0)
        {
            return BadRequest(new
            {
                message = "No file uploaded"
            });
        }
        
        var fileName = await _avatarService.UploadAvatarAsync(avatar);

        var login = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name);
        if (login == null)
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetUserByLoginAsync(login.Value);
        if (user == null)
        {
            return Unauthorized();
        }
        
        user.AvatarUrl = fileName;
        await _userRepository.UpdateAsync(user);
        
        return Ok(new
        {
            message = "Avatar setted",
            url = fileName
        });
    }
}