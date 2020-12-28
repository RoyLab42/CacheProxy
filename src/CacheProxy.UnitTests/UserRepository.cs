using System;
using System.Threading.Tasks;

namespace RoyLab.CacheProxy.UnitTests
{
    public class UserRepository : IUserRepository
    {
        public async Task<string> GetUserNameAsync(Guid guid)
        {
            await Task.Delay(5000);
            return "RoyZhang";
        }
    }
}