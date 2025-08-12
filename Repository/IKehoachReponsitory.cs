using AspnetCoreMvcFull.Data;
using PLANMHE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PLANMHE.Repository
{
  public interface IKehoachRepository
  {
    Task AddPlanAsync(Plan plan, IEnumerable<int> userIds);
    Task<IEnumerable<User>> GetAllUsersAsync();
  }
}
