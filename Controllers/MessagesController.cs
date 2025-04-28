using Connectify.Db.Model;
using GachiHubBackend.Repositories;
using GachiHubBackend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GachiHubBackend.Controllers;

[Route("/Api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly MessagesRepository _messageRepository;
    
    private readonly IRepository<Room> _roomRepository;

    public MessagesController(MessagesRepository messageRepository, IRepository<Room> roomRepository)
    {
        _messageRepository = messageRepository;
        _roomRepository = roomRepository;
    }

    [HttpGet(nameof(GetMessagesByRoomId))]
    public async Task<IActionResult> GetMessagesByRoomId(int id, int page = 1, int pageSize = 10)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            return NotFound(new
            {
                message = "Room not found"
            });
        }
        
        var (totalCount, messages) = await _messageRepository.GetMessagesByRoomId(id, page, pageSize);
        
        return Ok(new
        {
            messages = messages,
            totalCount = totalCount,
        });
    }
}