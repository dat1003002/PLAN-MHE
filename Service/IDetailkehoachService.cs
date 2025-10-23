using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public interface IDetailkehoachService
  {
    Task<int> AddPlanAsync(Plan plan, IEnumerable<int> userIds);
    Task AddPlanCellsAsync(IEnumerable<PlanCell> planCells);
    Task<IEnumerable<PlanCell>> GetPlanCellsAsync(int planId);
    Task UpdatePlanCellAsync(PlanCell planCell);
    Task<LockRowsResult> LockRowsAsync(int planId, IEnumerable<int> selectedRows);
  }

  public class LockRowsResult
  {
    public List<List<object>> TableData { get; set; }
    public List<Dictionary<string, string>> Formats { get; set; }
    public List<Dictionary<string, bool>> LockedCells { get; set; }
  }
}
