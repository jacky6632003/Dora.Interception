using System.Threading.Tasks;

namespace WebApi_Dora_AOP.Service
{
    public interface ICustomerService
    {
        Task<string> GetAllAsync();
    }
}
