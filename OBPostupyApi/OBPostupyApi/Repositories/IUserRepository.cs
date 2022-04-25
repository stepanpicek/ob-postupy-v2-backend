using OBPostupyApi.Entities;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId);
    }
}
