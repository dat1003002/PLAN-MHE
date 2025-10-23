using System.Threading.Tasks;
using PLANMHE.Models;

namespace PLANMHE.Repository
{
  public interface IAuthRepository
  {
    Task<User> GetUserByUsernameAsync(string username);
    Task CreateUserAsync(User user);
    Task<User> GetUserByIdAsync(int userId);
  }
}
