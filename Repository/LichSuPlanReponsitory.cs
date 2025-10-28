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

    // GIỮ NGUYÊN: DÙNG CHO DETAIL & EXPORT
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

    // MỚI: CHỈ DÙNG CHO DANH SÁCH – KHÔNG TẢI PlanCells
    public IEnumerable<Plan> GetPlansForList(int pageNumber, int pageSize)
    {
      return _context.Plans
          .Include(p => p.Creator)
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
