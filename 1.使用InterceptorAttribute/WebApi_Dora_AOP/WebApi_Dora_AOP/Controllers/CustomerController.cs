using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApi_Dora_AOP.Service;

namespace WebApi_Dora_AOP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        public async Task<string> GetAllAsync()
        {
            Console.WriteLine("CustomerController執行中...");
            return await this._customerService.GetAllAsync();
        }
    }
}