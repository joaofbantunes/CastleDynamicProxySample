using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingMilitia.CastleDynamicProxySample.Caching
{
    public class MethodCacheConfiguration
    {
        public bool UseCache { get; set; }
        public string MethodId { get; set; }        
        public bool CacheNullValues { get; set; }
        public bool CacheEmptyCollectionValues { get; set; }
        public TimeSpan? Ttl { get; set; }

    }
}
