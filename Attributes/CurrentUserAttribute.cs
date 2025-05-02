using GachiHubBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GachiHubBackend.Attributes;

public class CurrentUserAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var login = context.HttpContext.User.Identity!.Name;
        if (string.IsNullOrEmpty(login))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var userRepository = (UserRepository)context.HttpContext.RequestServices.GetService(typeof(UserRepository))!;
        
        var user = await userRepository.GetUserByLoginAsync(login);
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        context.HttpContext.Items["CurrentUser"] = user;

        await next();
    }
}