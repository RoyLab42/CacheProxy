using System;

namespace RoyLab.CacheProxy
{
    public sealed class CacheOption<T>
    {
        public TimeSpan Timeout { get; set; }
    }
}