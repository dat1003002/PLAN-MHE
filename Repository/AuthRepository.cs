using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspnetCoreMvcFull.Data; // Đảm bảo namespace đúng cho ApplicationDbContext
using PLANMHE.Models;

namespace PLANMHE.Repository
{
  public class AuthRepository : IAuthRepository
  {
    private readonly ApplicationDbContext _context;

    public AuthRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
      return await _context.Users
          .Include(u => u.UserType)
          .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task CreateUserAsync(User user)
    {
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
      return await _context.Users
          .Include(u => u.UserType)
          .FirstOrDefaultAsync(u => u.Id == userId);
    }
  }
}
