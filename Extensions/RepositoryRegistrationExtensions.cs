using System.Reflection;
using Connectify.Db.Model;
using GachiHubBackend.Repositories;
using GachiHubBackend.Repositories.Interfaces;

namespace GachiHubBackend.Extensions;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var baseModelType = typeof(BaseModel);
        var repositoryInterfaceType = typeof(IRepository<>);
        var connectifyRepositoryType = typeof(ConnectifyRepository<>);

        var assembly = Assembly.GetAssembly(typeof(BaseModel));

        var modelTypes = assembly!.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && baseModelType.IsAssignableFrom(t));

        foreach (var modelType in modelTypes)
        {
            var repoInterface = repositoryInterfaceType.MakeGenericType(modelType);
            var repoImplementation = connectifyRepositoryType.MakeGenericType(modelType);
            services.AddScoped(repoInterface, repoImplementation);
        }

        return services;
    }
}