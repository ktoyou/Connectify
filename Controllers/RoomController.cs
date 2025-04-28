using Connectify.Db.Model;
using GachiHubBackend.Repositories;
using GachiHubBackend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GachiHubBackend.Controllers;

[Route("/Api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly RoomRepository _roomRepository;
    
    private readonly UserRepository _userRepository;
    
    private readonly IRepository<Message> _messageRepository;
    
    public RoomController(RoomRepository roomRepository, UserRepository userRepository, IRepository<Message> messagesRepository)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _messageRepository = messagesRepository;
    }

    [HttpGet(nameof(GetRooms))]
    public async Task<IActionResult> GetRooms(int pageNumber = 1, int pageSize = 10)
    {
        var (totalCount, rooms) = await _roomRepository.GetPageAsync(pageNumber, pageSize);
        return Ok (new 
        {
            rooms = rooms,
            totalCount = totalCount
        });
    }

    [HttpPost(nameof(CreateRoom))]
    public async Task<IActionResult> CreateRoom(string title)
    {
        var room = await _roomRepository.GetRoomByTitleAsync(title);
        if (room != null)
        {
            return BadRequest(new
            {
                message = "Room already exists"
            });
        }

        var login = HttpContext.User.Claims.ToList()[0].Value;
        var user = await _userRepository.GetUserByLoginAsync(login);
        if (user == null)
        {
            return Unauthorized();
        }
        
        await _roomRepository.AddAsync(new Room()
        {
            Title = title,
            Owner = user,
        });

        return Ok(new
        {
            message = "Room created"
        });
    }
}