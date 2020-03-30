using System;
using System.Threading.Tasks;
using WebApi_Dora_AOP.Common;

namespace WebApi_Dora_AOP.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        //寫上Order屬性會依照順序，1先執行之後是2，依此類推，
        //如果都不寫Order屬性，掛在越下面的InterceptorAttribute越優先執行
        [LogInterceptor(Order = 1)]
        [Log(Order = 2)]
        public Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerRepository執行中...");
            return Task.FromResult("Get All Customers");
        }
    }
}
