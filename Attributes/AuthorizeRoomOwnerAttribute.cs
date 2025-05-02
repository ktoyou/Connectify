using Connectify.Db.Model;
using GachiHubBackend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GachiHubBackend.Attributes;

public class AuthorizeRoomOwnerAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var login = context.HttpContext.User.Identity?.Name;
        if (login == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var roomId = context.RouteData.Values["roomId"]?.ToString();
        if (string.IsNullOrEmpty(roomId))
        {
            context.Result = new BadRequestResult();
            return;
        }

        var roomRepository = (IRepository<Room>)context.HttpContext.RequestServices.GetService(typeof(IRepository<Room>))!;
        var room = await roomRepository.GetByIdAsync(int.Parse(roomId));

        if (room == null || room.Owner.Login != login)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}