using AspnetCoreMvcFull.Data;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Repository
{
  public class UserRepository : IUserRepository
  {
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
      return await _context.Users.Include(u => u.UserType).ToListAsync();
    }

    public async Task<User> GetByIdAsync(int id)
    {
      return await _context.Users.Include(u => u.UserType).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddAsync(User user)
    {
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
      _context.Users.Update(user);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
      var user = await GetByIdAsync(id);
      if (user != null)
      {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
      }
    }
  }
}
