using AspnetCoreMvcFull.Data;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
using PLANMHE.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Repositories
{
  public class UserTypeRepository : IUserTypeRepository
  {
    private readonly ApplicationDbContext _context;

    public UserTypeRepository(ApplicationDbContext context)
    {
      _context = context;
    }


    public async Task<IEnumerable<UserType>> GetAllAsync()
    {
      return await _context.UserTypes.ToListAsync();
    }

    public async Task<UserType> GetByIdAsync(int id)
    {
      Console.WriteLine($"Querying UserType with ID: {id}");
      var userType = await _context.UserTypes.FindAsync(id);
      Console.WriteLine($"Found UserType: {userType?.Id}");
      return userType;
    }

    public async Task AddAsync(UserType userType)
    {
      await _context.UserTypes.AddAsync(userType);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserType userType)
    {
      _context.UserTypes.Update(userType);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserType userType)
    {
      _context.UserTypes.Remove(userType);
      await _context.SaveChangesAsync();
    }
  }

}
