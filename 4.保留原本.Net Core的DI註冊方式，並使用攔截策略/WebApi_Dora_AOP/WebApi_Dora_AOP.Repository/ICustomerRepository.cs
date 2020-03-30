using System.Threading.Tasks;

namespace WebApi_Dora_AOP.Repository
{
    public interface ICustomerRepository
    {
        Task<string> GetAllAsync();
    }
}
