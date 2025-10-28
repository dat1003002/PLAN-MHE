using System.Collections.Generic;
using PLANMHE.Models;

namespace PLANMHE.Repository
{
  public interface ILichSuPlanReponsitory
  {
    // GIỮ NGUYÊN CÁC HÀM CŨ
    IEnumerable<Plan> GetAllPlans();     // Dùng cho Detail, Export
    Plan GetPlanById(int id);            // Dùng cho Detail, Export

    // THÊM MỚI: DÀNH RIÊNG CHO DANH SÁCH (KHÔNG TẢI PlanCells)
    IEnumerable<Plan> GetPlansForList(int pageNumber, int pageSize);
    int GetTotalPlanCount();
  }
}
