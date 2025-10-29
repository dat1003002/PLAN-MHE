using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
using AspnetCoreMvcFull.Data;

namespace PLANMHE.Repository
{
  public class LichSuPlanReponsitory : ILichSuPlanReponsitory
  {
    private readonly ApplicationDbContext _context;
    private const int PageSize = 10;

    public LichSuPlanReponsitory(ApplicationDbContext context)
    {
      _context = context;
    }

    // DÙNG CHO DETAIL & EXPORT
    public IEnumerable<Plan> GetAllPlans()
    {
      return _context.Plans
          .Include(p => p.Creator)
          .Include(p => p.PlanCells)
          .ToList();
    }

    public Plan GetPlanById(int id)
    {
      return _context.Plans
          .Include(p => p.Creator)
          .Include(p => p.PlanCells)
          .FirstOrDefault(p => p.Id == id);
    }

    // MỚI: DÀNH CHO DANH SÁCH – TẢI THÊM USER THỰC HIỆN
    public IEnumerable<Plan> GetPlansForList(int pageNumber, int pageSize)
    {
      return _context.Plans
          .AsNoTracking() // Tối ưu: không theo dõi (read-only)
          .Include(p => p.Creator)
          .Include(p => p.UserPlans)
              .ThenInclude(up => up.User) // Tải User từ bảng trung gian
          .OrderByDescending(p => p.StartDate)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();
    }

    public int GetTotalPlanCount()
    {
      return _context.Plans.Count();
    }
  }
}
