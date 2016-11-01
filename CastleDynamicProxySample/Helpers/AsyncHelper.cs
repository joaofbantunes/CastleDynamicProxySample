using System;
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

        public static async Task AwaitTaskWithFinally(Task actualReturnValue, Action finallyAction)
        {
            try
            {
                await actualReturnValue;
            }
            finally
            {
                finallyAction();
            }
        }

        public static async Task<T> AwaitTaskWithFinallyAndGetResult<T>(Task<T> actualReturnValue, Action finallyAction)
        {
            try
            {
                return await actualReturnValue;
            }
            finally
            {
                finallyAction();
            }
        }
    }
}
