using PLANMHE.Repository;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PLANMHE.Service
{
  public class DashboardsService : IDashboardsService
  {
    private readonly IDashboardsRepository _dashboardsRepository;

    public DashboardsService(IDashboardsRepository dashboardsRepository)
    {
      _dashboardsRepository = dashboardsRepository;
    }

    public async Task<List<Dictionary<string, object>>> GetTop5RecentPlansWithStatusAsync()
    {
      return await _dashboardsRepository.GetTop5RecentPlansWithStatusAsync();
    }
  }
}
