using PLANMHE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetCoreMvcFull.Data;

namespace PLANMHE.Repository
{
  public class KehoachRepository : IKehoachRepository
  {
    private readonly ApplicationDbContext _context;

    public KehoachRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<int> AddPlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      plan.CreatedDate = DateTime.UtcNow; // Đảm bảo CreatedDate được gán
      _context.Plans.Add(plan);
      await _context.SaveChangesAsync();

      foreach (var userId in userIds)
      {
        _context.UserPlans.Add(new UserPlan { PlanId = plan.Id, UserId = userId });
      }
      await _context.SaveChangesAsync();
      return plan.Id;
    }

    public async Task UpdatePlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      var existingPlan = await _context.Plans.FindAsync(plan.Id);
      if (existingPlan == null)
      {
        throw new Exception($"Plan with ID {plan.Id} does not exist.");
      }

      existingPlan.Name = plan.Name;
      existingPlan.Description = plan.Description;
      existingPlan.StartDate = plan.StartDate;
      existingPlan.EndDate = plan.EndDate;
      existingPlan.Status = plan.Status;
      existingPlan.CreatedBy = plan.CreatedBy; // Giữ nguyên CreatedBy
      existingPlan.CreatedDate = plan.CreatedDate; // Giữ nguyên CreatedDate

      var existingUserPlans = _context.UserPlans.Where(up => up.PlanId == plan.Id);
      _context.UserPlans.RemoveRange(existingUserPlans);

      foreach (var userId in userIds)
      {
        _context.UserPlans.Add(new UserPlan { PlanId = plan.Id, UserId = userId });
      }

      await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<int>> GetPlanUsersAsync(int planId)
    {
      return await _context.UserPlans
          .Where(up => up.PlanId == planId)
          .Select(up => up.UserId)
          .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _context.Users.ToListAsync();
    }

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
      return await _context.Plans.Include(p => p.Creator).ToListAsync();
    }

    public async Task DeletePlanAsync(int planId)
    {
      var plan = await _context.Plans.FindAsync(planId);
      if (plan != null)
      {
        _context.Plans.Remove(plan);
        await _context.SaveChangesAsync();
      }
    }
  }
}
