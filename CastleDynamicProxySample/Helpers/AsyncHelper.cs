using System.Reflection;
using System.Threading.Tasks;

namespace CodingMilitia.CastleDynamicProxySample.Helpers
{
    public static class AsyncHelper
    {
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }
    }
}
