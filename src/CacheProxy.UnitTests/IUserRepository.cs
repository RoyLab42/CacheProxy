using System;
using System.Threading.Tasks;

namespace RoyLab.CacheProxy.UnitTests
{
    [CacheIt]
    public interface IUserRepository
    {
        [EnableCache(DefaultTimeoutInTicks = TimeSpan.TicksPerHour * 6)]
        Task<string> GetUserNameAsync(Guid guid);
    }
}