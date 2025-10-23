using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Repository
{
  public interface IDetailkehoachReposive
  {
    Task<int> AddPlanAsync(Plan plan, IEnumerable<int> userIds);
    Task AddPlanCellsAsync(IEnumerable<PlanCell> planCells);
    Task<IEnumerable<PlanCell>> GetPlanCellsAsync(int planId);
    Task UpdatePlanCellAsync(PlanCell planCell);
    Task UpdatePlanCellsAsync(IEnumerable<PlanCell> planCells);
  }
}
