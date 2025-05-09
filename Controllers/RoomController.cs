﻿using Connectify.Db.Model;
using FluentValidation;
using GachiHubBackend.Attributes;
using GachiHubBackend.Hubs;
using GachiHubBackend.Hubs.Enums;
using GachiHubBackend.Repositories;
using GachiHubBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Controllers;

[Route("/Api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly RoomRepository _roomRepository;
    
    private readonly IHubContext<RoomHub> _roomHubContext;
    
    private readonly IValidator<Room> _roomValidator;
    
    private readonly IJanusService _janusService;
    
    public RoomController(RoomRepository roomRepository, IHubContext<RoomHub> roomHubContext, IValidator<Room> roomValidator, IJanusService janusService)
    {
        _roomRepository = roomRepository;
        _roomHubContext = roomHubContext;
        _roomValidator = roomValidator;
        _janusService = janusService;
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

    [AuthorizeRoomOwner]
    [HttpPost(nameof(RemoveRoom))]
    public async Task<IActionResult> RemoveRoom(int roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            return NotFound(new
            {
                message = "Room not found"
            });
        }

        await _roomRepository.DeleteAsync(room);
        return Ok(new
        {
            message = "Room removed"
        });
    }

    [HttpPost(nameof(CreateRoom))]
    [CurrentUser]
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

        var user = HttpContext.Items["CurrentUser"] as User;
        var newRoom = new Room()
        {
            Title = title,
            Owner = user!,
        };
        
        var validationResult = await _roomValidator.ValidateAsync(newRoom);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList()
            });
        }
        
        await _roomRepository.AddAsync(newRoom);
        var session = await _janusService.CreateSessionAsync();
        var plugin = await _janusService.AttachToPluginAsync(session, "janus.plugin.audiobridge");
        await _janusService.CreateRoomAsync(session, plugin, newRoom.Id);
        
        await _roomHubContext.Clients.All.SendAsync(RoomHubEvent.CreatedRoom.ToString(), newRoom);

        return Ok(new
        {
            message = "Room created"
        });
    }
}