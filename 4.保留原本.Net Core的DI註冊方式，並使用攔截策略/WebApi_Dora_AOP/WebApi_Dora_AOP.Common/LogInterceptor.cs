using Dora.Interception;
using System;
using System.Threading.Tasks;

namespace WebApi_Dora_AOP.Common
{
    public class LogInterceptor
    {
        public async Task InvokeAsync(InvocationContext context)
        {
            Console.WriteLine($"[Log] {context.Target.GetType().Name} - {context.Method.Name} 執行前");

            await context.ProceedAsync();

            Console.WriteLine($"[Log] {context.Target.GetType().Name} - {context.Method.Name} 執行後");
        }
    }
}
