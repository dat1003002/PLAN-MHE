using PLANMHE.Models;
using PLANMHE.Repositories.Interfaces;
using PLANMHE.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLANMHE.Services
{
  public class UserTypeService : IUserTypeService
  {
    private readonly IUserTypeRepository _repository;

    public UserTypeService(IUserTypeRepository repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<UserType>> GetAllUserTypesAsync()
    {
      return await _repository.GetAllAsync();
    }

    public async Task<UserType> GetUserTypeByIdAsync(int id)
    {
      return await _repository.GetByIdAsync(id);
    }

    public async Task AddUserTypeAsync(UserType userType)
    {
      await _repository.AddAsync(userType);
    }

    public async Task UpdateUserTypeAsync(UserType userType)
    {
      var existingUserType = await _repository.GetByIdAsync(userType.Id);
      if (existingUserType == null)
      {
        throw new Exception("Loại người dùng không tồn tại!");
      }
      existingUserType.FullName = userType.FullName;
      existingUserType.Note = userType.Note;
      await _repository.UpdateAsync(existingUserType);
    }

    public async Task DeleteUserTypeAsync(int id)
    {
      var userType = await _repository.GetByIdAsync(id);
      if (userType == null)
      {
        throw new Exception("Loại người dùng không tồn tại!");
      }
      // Optional: Kiểm tra dependencies nếu cần
      // var hasDependencies = await _repository.HasDependenciesAsync(id);
      // if (hasDependencies)
      // {
      //     throw new Exception("Không thể xóa loại người dùng vì có người dùng liên quan!");
      // }
      await _repository.DeleteAsync(userType); // Pass entity đã fetch, tránh tạo stub
    }
  }
}
