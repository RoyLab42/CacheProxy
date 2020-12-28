using System;

namespace RoyLab.CacheProxy
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EnableCacheAttribute : Attribute
    {
        public long DefaultTimeoutInTicks
        {
            get => DefaultTimeout.Ticks;
            set => DefaultTimeout = new TimeSpan(value);
        }

        internal TimeSpan DefaultTimeout { get; private set; }
    }
}