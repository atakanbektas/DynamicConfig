using Config.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Config.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string mongoConn)
    {
        services.AddSingleton<IConfigRepository>(_ => new MongoConfigRepository(mongoConn));
        return services;
    }
}
