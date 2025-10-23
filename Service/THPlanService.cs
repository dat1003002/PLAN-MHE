using PLANMHE.Models;
using PLANMHE.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public class THPlanService : ITHPlanService
  {
    private readonly ITHPlanRepository _repository;

    public THPlanService(ITHPlanRepository repository)
    {
      _repository = repository;
    }

    public List<Plan> GetPlansByUserId(int userId, bool isAdmin)
    {
      if (isAdmin)
      {
        return _repository.GetAllPlans();
      }
      return _repository.GetPlansByUserId(userId);
    }

    public async Task<Plan> GetPlanById(int planId)
    {
      return await _repository.GetPlanById(planId);
    }

    public async Task<List<User>> GetAssignedUsersByPlanId(int planId)
    {
      return await _repository.GetAssignedUsersByPlanId(planId);
    }

    public async Task<List<PlanCell>> GetPlanCellsByPlanId(int planId)
    {
      return await _repository.GetPlanCellsByPlanId(planId);
    }

    public async Task UpdatePlanCellAsync(PlanCell planCell)
    {
      await _repository.UpdatePlanCellAsync(planCell);
    }

    public async Task AddRowAsync(int planId)
    {
      var planCells = await _repository.GetPlanCellsByPlanId(planId);
      int newRowId = planCells.Any() ? planCells.Max(pc => pc.RowId) + 1 : 1;
      await _repository.AddRowAsync(planId, newRowId);
    }

    public async Task AddColumnAsync(int planId)
    {
      var planCells = await _repository.GetPlanCellsByPlanId(planId);
      int newColumnId = planCells.Any() ? planCells.Max(pc => pc.ColumnId) + 1 : 1;
      await _repository.AddColumnAsync(planId, newColumnId);
    }

    public async Task DeleteRowAsync(int planId, int rowId)
    {
      await _repository.DeleteRowAsync(planId, rowId);
    }

    public async Task DeleteColumnAsync(int planId, int columnId)
    {
      await _repository.DeleteColumnAsync(planId, columnId);
    }

    public async Task ConfirmPlanAsync(int planId)
    {
      await _repository.ConfirmPlanAsync(planId);
    }
  }
}
