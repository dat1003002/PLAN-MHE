using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public interface ITHPlanService
  {
    List<Plan> GetPlansByUserId(int userId, bool isAdmin);
    Task<Plan> GetPlanById(int planId);
    Task<List<User>> GetAssignedUsersByPlanId(int planId);
    Task<List<PlanCell>> GetPlanCellsByPlanId(int planId);
    Task UpdatePlanCellAsync(PlanCell planCell);
    Task AddRowAsync(int planId);
    Task AddColumnAsync(int planId);
    Task DeleteRowAsync(int planId, int rowId);
    Task DeleteColumnAsync(int planId, int columnId);
    Task ConfirmPlanAsync(int planId);
  }
}
