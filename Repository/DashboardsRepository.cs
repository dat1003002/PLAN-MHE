using Microsoft.EntityFrameworkCore;
using AspnetCoreMvcFull.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PLANMHE.Repository
{
  public class DashboardsRepository : IDashboardsRepository
  {
    private readonly ApplicationDbContext _context;

    public DashboardsRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<Dictionary<string, object>>> GetTop5RecentPlansWithStatusAsync()
    {
      var plans = await _context.Plans
          .OrderByDescending(p => p.CreatedDate)
          .Take(5)
          .Select(p => new Dictionary<string, object>
          {
                    { "PlanId", p.Id },
                    { "PlanName", p.Name },
                    { "CreatedDate", p.CreatedDate ?? DateTime.UtcNow },
                    { "Status", p.Status ?? "Unknown" }
          })
          .ToListAsync();

      // Tính toán thống kê
      var completedCount = plans.Count(p => (string)p["Status"] == "Completed");
      var activeCount = plans.Count(p => (string)p["Status"] == "Active");
      var totalCount = plans.Count;

      // Thêm dòng tóm tắt vào cuối danh sách
      plans.Add(new Dictionary<string, object>
            {
                { "TotalPlans", totalCount },
                { "CompletedPlans", completedCount },
                { "ActivePlans", activeCount }
            });

      return plans;
    }
  }
}
