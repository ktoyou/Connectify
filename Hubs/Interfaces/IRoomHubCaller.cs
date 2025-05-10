using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace GachiHubBackend.Hubs.Interfaces;

public interface IRoomHubCaller
{
    IHubCallerClients Clients { get; }
    HubCallerContext Context { get; }
    IGroupManager Groups { get; }
}