using System;
using System.Threading.Tasks;
using WebApi_Dora_AOP.Repository;

namespace WebApi_Dora_AOP.Service
{
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
}
