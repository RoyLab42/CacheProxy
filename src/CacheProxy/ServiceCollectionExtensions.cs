using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RoyLab.CacheProxy
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// enable cache behavior for all interfaces registered before this call, which has <see cref="CacheItAttribute"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCacheProxy(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddMemoryCache();

            var cacheServiceDescriptors = services
                .Where(descriptor => descriptor.ServiceType.GetCustomAttribute<CacheItAttribute>() != null)
                .ToList();

            foreach (var descriptor in cacheServiceDescriptors)
            {
                var cachedServiceDescriptor = ServiceDescriptor.Describe(descriptor.ServiceType,
                    provider =>
                    {
                        var proxy = typeof(CacheProxy<>).MakeGenericType(descriptor.ServiceType)
                            .GetMethod("Create")?.Invoke(null, new object[]
                            {
                                provider,
                                descriptor.ImplementationType
                            });
                        return proxy;
                    }, descriptor.Lifetime);
                services.Remove(descriptor);
                services.Add(cachedServiceDescriptor);
            }

            return services;
        }
    }
}