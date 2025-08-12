using PLANMHE.Models;
using PLANMHE.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public interface IKehoachService
  {
    Task AddPlanAsync(Plan plan, IEnumerable<int> userIds);
    Task<IEnumerable<User>> GetAllUsersAsync();
  }
}
