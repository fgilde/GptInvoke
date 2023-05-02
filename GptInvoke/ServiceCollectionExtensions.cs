using System.Reflection;
using GptInvoke.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace GptInvoke;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGptActionInvoker(this IServiceCollection services, string key, params Assembly[] serviceImplementationAssemblies)
    {
        return services.AddGptActionInvoker(s => s.ApiKey = key, serviceImplementationAssemblies);
    }

    public static IServiceCollection AddGptActionInvoker(this IServiceCollection services, Action<GptActionInvokeSettings> config, params Assembly[] serviceImplementationAssemblies)
    {
        var settings = new GptActionInvokeSettings();
        config?.Invoke(settings);
        return services.AddGptActionInvoker(settings, serviceImplementationAssemblies);
    }

    public static IServiceCollection AddGptActionInvoker(this IServiceCollection services, GptActionInvokeSettings settings, params Assembly[] serviceImplementationAssemblies)
    {
        services.AddTransient(_ => settings);
        ServiceDescriptor invokerServiceDescriptor = new ServiceDescriptor(typeof(IGptActionInvoker), typeof(GptActionInvoker), settings.InvokerServiceLifetime);
        services.Add(invokerServiceDescriptor);
        //services.AddTransient<IGptActionInvoker, GptActionInvoker>();
        return services.RegisterAllImplementationsOf(new[] { typeof(IGptInvokableService) }, serviceImplementationAssemblies);
    }

}