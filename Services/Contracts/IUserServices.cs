using System.Security.Claims;
using System.Threading.Tasks;
using Models;
using Models.AdminModels;
using Models.Entities;

namespace Services.Contracts
{
    public interface IUserServices
    {
        Task<User> Authenticate(string username, string password);
        Task<User> RegisterUser(RegisterViewModel model);
        ClaimsIdentity GetSecurityClaims(User userInfo, string authenticationType = null);
    }
}