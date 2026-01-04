using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace mvmclean.backend.Application;

public static class Registrations
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection serviceCollection)
    {
        var asm = Assembly.GetExecutingAssembly();

        serviceCollection.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(asm));

        return serviceCollection;
    }

}