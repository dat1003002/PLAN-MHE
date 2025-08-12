using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Repositories.Interfaces
{
  public interface IUserTypeRepository
  {
    Task<IEnumerable<UserType>> GetAllAsync();
    Task<UserType> GetByIdAsync(int id);
    Task AddAsync(UserType userType);
    Task UpdateAsync(UserType userType);
    Task DeleteAsync(UserType userType); // Sửa: Nhận UserType thay vì int

    // Optional: Nếu cần kiểm tra dependencies
    // Task<bool> HasDependenciesAsync(int id);
  }
}
