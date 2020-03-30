using Dora.Interception;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi_Dora_AOP.Repository;
using WebApi_Dora_AOP.Service;

namespace WebApi_Dora_AOP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //加這行 Dora.Interception，使用 攔截策略
                .UseInterceptableServiceProvider(configure: ConfigureDora)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // 定義 攔截策略
        static void ConfigureDora(InterceptionBuilder interceptionBuilder)
        {
            interceptionBuilder
                .AddPolicy
                (
                    policyBuilder => policyBuilder
                        .For<LogInterceptorAttribute>
                        (
                            order: 1,
                            builder => builder.To<CustomerService>(target => target.IncludeMethod(service => service.GetAllAsync()))
                                .To<CustomerRepository>(target => target.IncludeMethod(repository => repository.GetAllAsync()))
                        )
                        .For<LogAttribute>
                        (
                            order: 2,
                            builder => builder.To<CustomerService>(target => target.IncludeMethod(service => service.GetAllAsync()))
                                .To<CustomerRepository>(target => target.IncludeMethod(repository => repository.GetAllAsync()))
                        )
                );
        }
    }
}
