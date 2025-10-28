using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Repository
{
  public interface IDashboardsRepository
  {
    Task<List<Dictionary<string, object>>> GetTop5RecentPlansWithStatusAsync();
  }
}
