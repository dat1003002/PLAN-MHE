  using System.Threading.Tasks;
using BCrypt.Net;
using PLANMHE.Models;
using PLANMHE.Repository;

namespace PLANMHE.Service
{
  public class AuthService : IAuthService
  {
    private readonly IAuthRepository _authRepository;

    public AuthService(IAuthRepository authRepository)
    {
      _authRepository = authRepository;
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {
      var user = await _authRepository.GetUserByUsernameAsync(username);
      if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
      {
        return null;
      }
      return user;
    }

    public async Task CreateAdminUserIfNotExistsAsync()
    {
      var adminUser = await _authRepository.GetUserByUsernameAsync("admin");
      if (adminUser == null)
      {
        var admin = new User
        {
          Username = "admin",
          Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
          FullName = "Administrator",
          Email = "admin@example.com",
          Sex = "Male",
          UserTypeId = 1
        };
        await _authRepository.CreateUserAsync(admin);
      }
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
      return await _authRepository.GetUserByIdAsync(userId);
    }
  }
}
