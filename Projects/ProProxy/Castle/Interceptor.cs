using System;
using Castle.DynamicProxy;

namespace ProProxy.Castle
{
    public class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Before target call {invocation.Method.Name}");
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Target exception {ex.Message}");
                throw;
            }
            finally
            {
                Console.WriteLine($"After target call {invocation.Method.Name}");
            }
        }
    }
}