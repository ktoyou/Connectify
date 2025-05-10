using System.Security.Claims;
using GachiHubBackend.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Hubs;

public class RoomHubCaller : IRoomHubCaller
{
    public IHubCallerClients Clients { get; }
    
    public HubCallerContext Context { get; }
    
    public IGroupManager Groups { get; }
    
    public RoomHubCaller(IHubCallerClients clients, HubCallerContext context, IGroupManager groups)
    {
        Clients = clients;
        Context = context;
        Groups = groups;
    }
}