using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public interface IKehoachService
  {
    Task<int> AddPlanAsync(Plan plan, IEnumerable<int> userIds);
    Task UpdatePlanAsync(Plan plan, IEnumerable<int> userIds);
    Task<IEnumerable<int>> GetPlanUsersAsync(int planId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<Plan>> GetAllPlansAsync();
    Task DeletePlanAsync(int planId);
  }
}
