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

      public LichSuPlanReponsitory(ApplicationDbContext context)
      {
        _context = context;
      }

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
    }
  }
