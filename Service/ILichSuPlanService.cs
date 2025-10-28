using System.Collections.Generic;
using PLANMHE.Models;

namespace PLANMHE.Service
{
  public interface ILichSuPlanService
  {
    IEnumerable<Plan> GetAllPlans();     // Dùng cho Detail, Export
    Plan GetPlanById(int id);            // Dùng cho Detail, Export

    // MỚI: DÀNH CHO DANH SÁCH
    IEnumerable<Plan> GetPlansForList(int pageNumber, int pageSize);
    int GetTotalPlanCount();
  }
}
