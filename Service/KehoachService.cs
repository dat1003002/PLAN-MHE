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

    public async Task AddPlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      await _repository.AddPlanAsync(plan, userIds);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _repository.GetAllUsersAsync();
    }
  }
}
