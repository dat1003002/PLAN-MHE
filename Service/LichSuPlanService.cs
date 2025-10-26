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

    public IEnumerable<Plan> GetAllPlans()
    {
      return _repository.GetAllPlans();
    }

    public Plan GetPlanById(int id)
    {
      return _repository.GetPlanById(id);
    }
  }
}
