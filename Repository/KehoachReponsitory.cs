using AspnetCoreMvcFull.Data;
using PLANMHE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PLANMHE.Repository
{
  public class KehoachRepository : IKehoachRepository
  {
    private readonly ApplicationDbContext _context;

    public KehoachRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddPlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      _context.Plans.Add(plan);
      await _context.SaveChangesAsync(); // Lưu Plan trước để có Id

      foreach (var userId in userIds)
      {
        _context.UserPlans.Add(new UserPlan { PlanId = plan.Id, UserId = userId });
      }
      await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _context.Users.ToListAsync();
    }
  }
}
