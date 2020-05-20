using System;
using System.Diagnostics;
using System.Reflection;
using Funx.Extensions;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Core;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerCoreServiceCollectionExtensions
    {
        public static IWrapperizerBuilder AddHandlers(
            this IWrapperizerBuilder builder,
            ServiceLifetime serviceLifetime = Transient,
            params Assembly[] assemblies)
        {
            if( !assemblies.SafeAny() )
                assemblies =  AppDomain.CurrentDomain.GetAssemblies();
            
            builder.ServiceCollection.AddMediatR(assemblies, configuration =>
            {
                configuration = serviceLifetime switch
                {
                    Singleton => configuration.AsSingleton(),
                    Scoped => configuration.AsScoped(),
                    _ => configuration.AsTransient()
                };
            });
            
            // ServiceRegistrar.AddRequiredServices(builder.ServiceCollection, mediatRServiceConfiguration);
            // ServiceRegistrar.AddMediatRClasses(builder.ServiceCollection, assemblies);

            builder.AddDomainEventManager<DomainEventCommandQueryManager>(serviceLifetime);
            builder.AddCommandQueryManger<DomainEventCommandQueryManager>(serviceLifetime);

            return builder;
        }

        public static IWrapperizerBuilder AddCommandQueryManger<T>(this IWrapperizerBuilder builder, ServiceLifetime serviceLifetime)
            where T : ICommandQueryManager
        {
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(ICommandQueryManager),
                    typeof(DomainEventCommandQueryManager), serviceLifetime));

            return builder;
        }

        public static IWrapperizerBuilder AddDomainEventManager<T>(
            this IWrapperizerBuilder builder, ServiceLifetime serviceLifetime = Transient)
            where T : IDomainEventManager
        {
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(IDomainEventManager),
                    typeof(DomainEventCommandQueryManager), serviceLifetime));
            return builder;
        }
    }
}
