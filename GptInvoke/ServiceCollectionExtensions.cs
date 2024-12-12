using System.Reflection;
using GptInvoke.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace GptInvoke;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAIActionInvoker(this IServiceCollection services, string key, params Assembly[] serviceImplementationAssemblies)
    {
        return services.AddOpenAIActionInvoker(s => {}, serviceImplementationAssemblies);
    }

    public static IServiceCollection AddOpenAIActionInvoker(this IServiceCollection services, Action<GptActionInvokeSettings> config, params Assembly[] serviceImplementationAssemblies)
    {
        var settings = new GptActionInvokeSettings();
        config?.Invoke(settings);
        services.Add(new ServiceDescriptor(typeof(IAIHandler), provider => new OpenAiHandler(settings.ApiKey, settings.Model), settings.InvokerServiceLifetime));
        return services.AddAIActionInvoker(settings, serviceImplementationAssemblies);
    }

    public static IServiceCollection AddAIActionInvoker<TAIHandler>(this IServiceCollection services,
        AIActionInvokeSettings settings,
        params Assembly[] serviceImplementationAssemblies)
        where TAIHandler : class, IAIHandler
    {
        services.Add(new ServiceDescriptor(typeof(IAIHandler), typeof(TAIHandler), settings.InvokerServiceLifetime));
        services.Add(new ServiceDescriptor(typeof(TAIHandler), typeof(TAIHandler), settings.InvokerServiceLifetime));
        return services.AddAIActionInvoker(settings, serviceImplementationAssemblies);
    }

    public static IServiceCollection AddAIActionInvoker<TAIHandler>(this IServiceCollection services,
        AIActionInvokeSettings settings,
        Func<IServiceProvider, TAIHandler> handlerFactory,
        params Assembly[] serviceImplementationAssemblies)
        where TAIHandler : class, IAIHandler
    {
        services.Add(new ServiceDescriptor(typeof(IAIHandler), handlerFactory, settings.InvokerServiceLifetime));
        services.Add(new ServiceDescriptor(typeof(TAIHandler), handlerFactory, settings.InvokerServiceLifetime));
        return services.AddAIActionInvoker(settings, serviceImplementationAssemblies);
    }

    public static IServiceCollection AddAIActionInvoker<TAIHandler>(this IServiceCollection services,
        AIActionInvokeSettings settings,
        TAIHandler handler,
        params Assembly[] serviceImplementationAssemblies)
        where TAIHandler : class, IAIHandler
    {
        return services.AddAIActionInvoker(settings, p => handler);
    }

    public static IServiceCollection AddAIActionInvoker(this IServiceCollection services, AIActionInvokeSettings settings, params Assembly[] serviceImplementationAssemblies)
    {
        services.AddTransient(_ => settings);
        services.Add(new ServiceDescriptor(typeof(IAIActionInvoker), typeof(AIActionInvoker), settings.InvokerServiceLifetime));
        //services.AddTransient<IGptActionInvoker, GptActionInvoker>();
        return services.RegisterAllImplementationsOf(new[] { typeof(IAIInvokableService) }, serviceImplementationAssemblies);
    }

}