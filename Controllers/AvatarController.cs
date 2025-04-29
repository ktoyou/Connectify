using System.Security.Claims;
using GachiHubBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GachiHubBackend.Controllers;

[Route("/Api/[controller]")]
[Authorize]
public class AvatarController : ControllerBase
{
    private readonly UserRepository _userRepository;
    
    public AvatarController(UserRepository userRepository)
    {
        _userRepository = userRepository;
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
        
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
        Directory.CreateDirectory(uploadFolder);

        var fileName = Guid.NewGuid() + Path.GetExtension(avatar.FileName);
        var filePath = Path.Combine(uploadFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatar.CopyToAsync(stream);
        }

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