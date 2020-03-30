using System;
using System.Threading.Tasks;
using WebApi_Dora_AOP.Common;
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
}
