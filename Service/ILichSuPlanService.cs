using System.Collections.Generic;
using PLANMHE.Models;

namespace PLANMHE.Service
{
  public interface ILichSuPlanService
  {
    IEnumerable<Plan> GetAllPlans();
    Plan GetPlanById(int id);
  }
}
