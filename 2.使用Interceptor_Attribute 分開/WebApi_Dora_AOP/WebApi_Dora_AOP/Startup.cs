using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi_Dora_AOP.Repository;
using WebApi_Dora_AOP.Service;

namespace WebApi_Dora_AOP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInterception(); //���U�d�I��
            services.AddTransientInterceptable<ICustomerService, CustomerService>(); //���U�n�d�I�����O
            services.AddTransientInterceptable<ICustomerRepository, CustomerRepository>();
            //services.AddTransient<ICustomerService, CustomerService>(); //���Φۤv�A���U
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
