using System;
using System.Threading.Tasks;
using WebApi_Dora_AOP.Common;

namespace WebApi_Dora_AOP.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        [LogInterceptor]
        public Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerRepository執行中...");
            return Task.FromResult("Get All Customers");
        }
    }
}
