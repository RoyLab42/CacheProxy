using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RoyLab.CacheProxy
{
    public class CacheProxy<T> : DispatchProxy where T : class
    {
        private T _implementation;
        private IMemoryCache _memoryCache;
        private IOptions<CacheOption<T>> _options;

        public static T Create(IServiceProvider provider, Type implementationType)
        {
            object proxy = Create<T, CacheProxy<T>>();
            var p = (CacheProxy<T>) proxy;

            p._implementation = (T) ActivatorUtilities.GetServiceOrCreateInstance(provider, implementationType);
            p._memoryCache = provider.GetRequiredService<IMemoryCache>();
            p._options = provider.GetRequiredService<IOptions<CacheOption<T>>>();

            return (T) proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var enableCacheAttribute = targetMethod.GetCustomAttribute<EnableCacheAttribute>();
            if (enableCacheAttribute == null)
            {
                return targetMethod.Invoke(_implementation, args);
            }

            var key = $"{nameof(T)}.{targetMethod.Name}.{string.Join(".", args)}";
            if (_memoryCache.TryGetValue(key, out var result))
            {
                return result;
            }

            result = targetMethod.Invoke(_implementation, args);

            var cacheTimeout = _options.Value.Timeout;
            if (cacheTimeout == default)
            {
                cacheTimeout = enableCacheAttribute.DefaultTimeout;
            }

            if (cacheTimeout == default)
            {
                cacheTimeout = TimeSpan.FromDays(1);
            }

            if (result is Task task)
            {
                task.ContinueWith((t, state) =>
                {
                    if (t.Status != TaskStatus.RanToCompletion)
                    {
                        return;
                    }

                    var (c, k, timeout) = ((IMemoryCache, string, TimeSpan)) state;
                    c.Set(k, t, timeout);
                }, (_memoryCache, key, cacheTimeout));
            }
            else
            {
                _memoryCache.Set(key, result, cacheTimeout);
            }

            return result;
        }
    }
}