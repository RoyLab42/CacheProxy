using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace RoyLab.CacheProxy.UnitTests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void Test1()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddCacheProxy();
            var provider = services.BuildServiceProvider();

            var proxy = provider.GetService<IUserRepository>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var userName = await proxy.GetUserNameAsync(Guid.Empty);
            stopwatch.Stop();
            _testOutputHelper.WriteLine($"get {userName}, time spent {stopwatch.ElapsedMilliseconds}");
            stopwatch.Reset();
            var userNameFromCache = await proxy.GetUserNameAsync(Guid.Empty);
            stopwatch.Stop();
            _testOutputHelper.WriteLine($"get {userNameFromCache}, time spent {stopwatch.ElapsedMilliseconds}");

            proxy = provider.GetService<IUserRepository>();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            userNameFromCache = await proxy.GetUserNameAsync(Guid.Empty);
            stopwatch.Stop();
            _testOutputHelper.WriteLine($"get {userNameFromCache}, time spent {stopwatch.ElapsedMilliseconds}");
            stopwatch.Reset();
            userNameFromCache = await proxy.GetUserNameAsync(Guid.Empty);
            stopwatch.Stop();
            _testOutputHelper.WriteLine($"get {userNameFromCache}, time spent {stopwatch.ElapsedMilliseconds}");
        }

        [Fact]
        public void TestImplementationFactoryPerformance()
        {
            var services = new ServiceCollection();
            services.AddTransient<IUserRepository, UserRepository>();
            // services.AddSingleton<IUserRepository, UserRepository>();
            services.AddCacheProxy();
            var provider = services.BuildServiceProvider();

            provider.GetService<IUserRepository>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (var i = 0; i < 100; i++)
            {
                provider.GetService<IUserRepository>();
            }

            stopwatch.Stop();
            _testOutputHelper.WriteLine($"object creation takes: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}