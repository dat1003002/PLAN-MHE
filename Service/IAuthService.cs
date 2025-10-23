using System.Threading.Tasks;
using PLANMHE.Models;

namespace PLANMHE.Service
{
  public interface IAuthService
  {
    Task<User> AuthenticateAsync(string username, string password);
    Task CreateAdminUserIfNotExistsAsync();
    Task<User> GetUserByIdAsync(int userId);
  }
}
