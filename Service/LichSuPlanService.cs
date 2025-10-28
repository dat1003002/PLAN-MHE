using System.Collections.Generic;
using PLANMHE.Models;
using PLANMHE.Repository;

namespace PLANMHE.Service
{
  public class LichSuPlanService : ILichSuPlanService
  {
    private readonly ILichSuPlanReponsitory _repository;

    public LichSuPlanService(ILichSuPlanReponsitory repository)
    {
      _repository = repository;
    }

    // GIỮ NGUYÊN
    public IEnumerable<Plan> GetAllPlans()
    {
      return _repository.GetAllPlans();
    }

    public Plan GetPlanById(int id)
    {
      return _repository.GetPlanById(id);
    }

    // MỚI: DÀNH CHO DANH SÁCH
    public IEnumerable<Plan> GetPlansForList(int pageNumber, int pageSize)
    {
      return _repository.GetPlansForList(pageNumber, pageSize);
    }

    public int GetTotalPlanCount()
    {
      return _repository.GetTotalPlanCount();
    }
  }
}
