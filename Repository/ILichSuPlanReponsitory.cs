using System.Collections.Generic;
using PLANMHE.Models;

namespace PLANMHE.Repository
{
  public interface ILichSuPlanReponsitory
  {
    IEnumerable<Plan> GetAllPlans();
    Plan GetPlanById(int id);
  }
}
