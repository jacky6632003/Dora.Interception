參考: https://www.cnblogs.com/artech/p/dora-interception-3-01.html

作法1 InterceptorAttribute

1.在 Common專案，安裝 NuGet套件: Dora.Interception 3.0.0 (類別庫.Net Standard 版本要 2.1)

2.在 Common專案，建立 LogInterceptorAttribute.cs

//[AttributeUsage(AttributeTargets.Method)]
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

3.建立 CustomerRepository.cs

public class CustomerRepository : ICustomerRepository
    {
        public Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerRepository執行中...");
            return Task.FromResult("Get All Customers");
        }
    }

4.建立 CustomerService.cs

public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        public async Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerService執行中...");
            return await this._customerRepository.GetAllAsync();
        }
    }

5.修改 Startup.cs

public void ConfigureServices(IServiceCollection services)
        {
            services.AddInterception(); //註冊攔截器
            services.AddTransientInterceptable<ICustomerService, CustomerService>(); //註冊要攔截的類別
            services.AddTransientInterceptable<ICustomerRepository, CustomerRepository>();
            //services.AddTransient<ICustomerService, CustomerService>(); //不用自己再註冊
            services.AddControllers();
        }

==========================================================================================================

作法2 把 Interceptor 跟 Attribute 分開

1.在 Common專案，建立 LogInterceptor.cs

public class LogInterceptor
    {
        public async Task InvokeAsync(InvocationContext context)
        {
            Console.WriteLine($"[Log] {context.Target.GetType().Name} - {context.Method.Name} 執行前");

            await context.ProceedAsync();

            Console.WriteLine($"[Log] {context.Target.GetType().Name} - {context.Method.Name} 執行後");
        }
    }


2.在 Common專案，建立 LogAttribute.cs

//[AttributeUsage(AttributeTargets.Method)]
    public class LogAttribute : InterceptorAttribute
    {
        public override void Use(IInterceptorChainBuilder builder)
        {
            builder.Use<LogInterceptor>(Order);
        }
    }

3.混用兩個InterceptorAttribute

public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        //寫上Order屬性會依照順序，1先執行之後是2，依此類推，
        //如果都不寫Order屬性，掛在越下面的InterceptorAttribute越優先執行
        [LogInterceptor(Order = 1)]
        [Log(Order = 2)]
        public async Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerService執行中...");
            return await this._customerRepository.GetAllAsync();
        }
    }

==========================================================================================================

作法3，保留原本.Net Core的DI註冊方式

1.調整 Program.cs

public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //加這行 Dora.Interception
                .UseInterceptableServiceProvider()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }


2.調整 Startup.cs

public void ConfigureServices(IServiceCollection services)
        {
            services.AddInterception(); //註冊攔截器

            // Program.cs 使用 Host.CreateDefaultBuilder(args).UseInterceptableServiceProvider()
            // 之後就能使用原本.Net Core的DI註冊方式
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();

            services.AddControllers();
        }

3.混用兩個InterceptorAttribute

public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        //寫上Order屬性會依照順序，1先執行之後是2，依此類推，
        //如果都不寫Order屬性，掛在越下面的InterceptorAttribute越優先執行
        [LogInterceptor(Order = 1)]
        [Log(Order = 2)]
        public async Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerService執行中...");
            return await this._customerRepository.GetAllAsync();
        }
    }

==========================================================================================================

作法4，保留原本.Net Core的DI註冊方式，並使用攔截策略

1.調整 Program.cs

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

2.調整 Program.cs

public void ConfigureServices(IServiceCollection services)
        {
	    //這邊不用註冊攔截器
            //services.AddInterception(); //註冊攔截器

            // Program.cs 使用 Host.CreateDefaultBuilder(args).UseInterceptableServiceProvider()
            // 之後就能使用原本.Net Core的DI註冊方式
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();

            services.AddControllers();
        }

3.使用到攔截器的 Service 跟 Repository 不用掛上 InterceptorAttribute

 public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _CustomerRepository;
        public CustomerService(ICustomerRepository CustomerRepository)
        {
            this._CustomerRepository = CustomerRepository;
        }

	//不用掛，改由 Program.cs 定義 攔截策略
        //[LogInterceptor(Order = 1)]
        //[Log(Order = 2)]
        public async Task<string> GetService() 
        {
            Console.WriteLine("GetService執行中...");
            return await this._CustomerRepository.GetRepository();
        }
    }

4.可以把 Common專案 LogAttribute、LogInterceptor、LogInterceptorAttribute 這幾個的檔案刪除，解除安裝 Dora.Interception 3.0.0，
  改在 Controller專案 安裝 Dora.Interception 3.0.0，而將三個檔LogAttribute、LogInterceptor、LogInterceptorAttribute也改定義在 Controller專案