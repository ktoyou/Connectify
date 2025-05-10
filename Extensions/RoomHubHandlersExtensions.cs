using GachiHubBackend.Hubs.Interfaces;

namespace GachiHubBackend.Extensions;

public static class RoomHubHandlersExtensions
{
    public static IServiceCollection AddRoomHubHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IRoomHubHandler>()
            .AddClasses(classes => classes.AssignableTo<IRoomHubHandler>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.AddScoped<IDictionary<string, IRoomHubHandler>>(provider =>
        {
            var handlers = provider.GetServices<IRoomHubHandler>();
            return handlers.ToDictionary(handler => handler.GetType().Name, handler => handler);
        });

        return services;
    }
}