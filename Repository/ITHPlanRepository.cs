using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Repository
{
  public interface ITHPlanRepository
  {
    List<Plan> GetPlansByUserId(int userId);
    List<Plan> GetAllPlans();
    Task<Plan> GetPlanById(int planId);
    Task<List<User>> GetAssignedUsersByPlanId(int planId);
    Task<List<PlanCell>> GetPlanCellsByPlanId(int planId);
    Task UpdatePlanCellAsync(PlanCell planCell);
    Task AddRowAsync(int planId, int rowId);
    Task AddColumnAsync(int planId, int columnId);
    Task DeleteRowAsync(int planId, int rowId);
    Task DeleteColumnAsync(int planId, int columnId);
    Task ConfirmPlanAsync(int planId);
  }
}
