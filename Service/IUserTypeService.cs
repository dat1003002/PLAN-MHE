using PLANMHE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Services.Interfaces
{
  public interface IUserTypeService
  {
    Task<IEnumerable<UserType>> GetAllUserTypesAsync();
    Task<UserType> GetUserTypeByIdAsync(int id);
    Task AddUserTypeAsync(UserType userType);
    Task UpdateUserTypeAsync(UserType userType);
    Task DeleteUserTypeAsync(int id);
  }
}
