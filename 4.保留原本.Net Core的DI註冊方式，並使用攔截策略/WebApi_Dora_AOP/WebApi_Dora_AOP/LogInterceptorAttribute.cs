using Dora.Interception;
using System;
using System.Threading.Tasks;

namespace WebApi_Dora_AOP
{
    public class LogInterceptorAttribute : InterceptorAttribute
    {
        public async Task InvokeAsync(InvocationContext context)
        {
            Console.WriteLine($"[LogInterceptor] {context.Target.GetType().Name} - {context.Method.Name} 執行前");

            await context.ProceedAsync();

            Console.WriteLine($"[LogInterceptor] {context.Target.GetType().Name} - {context.Method.Name} 執行後");
        }

        public override void Use(IInterceptorChainBuilder builder)
        {
            builder.Use(this, Order);
        }
    }
}
