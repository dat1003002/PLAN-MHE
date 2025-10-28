using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public interface IDashboardsService
  {
    Task<List<Dictionary<string, object>>> GetTop5RecentPlansWithStatusAsync();
  }
}
