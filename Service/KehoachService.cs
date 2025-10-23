using PLANMHE.Models;
using PLANMHE.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public class KehoachService : IKehoachService
  {
    private readonly IKehoachRepository _repository;

    public KehoachService(IKehoachRepository repository)
    {
      _repository = repository;
    }

    public async Task<int> AddPlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      return await _repository.AddPlanAsync(plan, userIds);
    }

    public async Task UpdatePlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      await _repository.UpdatePlanAsync(plan, userIds);
    }

    public async Task<IEnumerable<int>> GetPlanUsersAsync(int planId)
    {
      return await _repository.GetPlanUsersAsync(planId);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _repository.GetAllUsersAsync();
    }

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
      return await _repository.GetAllPlansAsync();
    }

    public async Task DeletePlanAsync(int planId)
    {
      await _repository.DeletePlanAsync(planId);
    }
  }
}
