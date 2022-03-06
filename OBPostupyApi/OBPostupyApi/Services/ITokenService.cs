using OBPostupyApi.Entities;
using System;
using System.Threading.Tasks;

namespace OBPostupyApi.Services
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user, DateTime expiration);
    }
}
