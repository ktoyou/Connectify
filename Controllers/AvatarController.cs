using System.Security.Claims;
using Connectify.Db.Model;
using GachiHubBackend.Attributes;
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
    [CurrentUser]
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
        var user = HttpContext.Items["CurrentUser"] as User;
        
        user!.AvatarUrl = fileName;
        await _userRepository.UpdateAsync(user);
        
        return Ok(new
        {
            message = "Avatar setted",
            url = fileName
        });
    }
}