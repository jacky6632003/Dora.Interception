using System;
using System.Threading.Tasks;

namespace WebApi_Dora_AOP.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        public Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerRepository執行中...");
            return Task.FromResult("Get All Customers");
        }
    }
}
