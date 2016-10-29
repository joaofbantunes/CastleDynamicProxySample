using System;

namespace CodingMilitia.CastleDynamicProxySample.Caching.Configuration
{
    /// <summary>
    /// Class that represents an attribute that should be used to configure the caching of a method result.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheInterceptorConfigurationAttribute : Attribute
    {
        public bool UseCache { get; set; }
        public string MethodId { get; set; }
        public bool CacheNullValues { get; set; }
        public bool CacheEmptyCollectionValues { get; set; }
    }
}