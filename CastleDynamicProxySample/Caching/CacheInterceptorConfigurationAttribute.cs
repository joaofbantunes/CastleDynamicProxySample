using System;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    /// <summary>
    /// Class that represents an attribute that should be used to configure the caching of a method result.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheInterceptorConfigurationAttribute : Attribute
    {
        public bool UseCache { get; set; }
        public string MethodKey { get; set; }
        public string[] ArgumentsToIgnoreOnKeyCreation { get; set; }
        public bool CacheNullValues { get; set; }
        public bool CacheEmptyCollectionValues { get; set; }

        public CacheInterceptorConfigurationAttribute()
        {
        }

    }
}