using PLANMHE.Models;
using PLANMHE.Repository;
using PLANMHE.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;
using AspnetCoreMvcFull.Data;

namespace PLANMHE.Services
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;

    public UserService(IUserRepository userRepository, ApplicationDbContext context)
    {
      _userRepository = userRepository;
      _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _userRepository.GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
      return await _userRepository.GetByIdAsync(id);
    }

    public async Task AddUserAsync(User user)
    {
      // Kiểm tra Username trùng lặp
      var existingUsers = await _userRepository.GetAllAsync();
      if (existingUsers.Any(u => u.Username == user.Username))
      {
        throw new Exception("Tên đăng nhập đã tồn tại!");
      }
      // BỎ kiểm tra Email trùng lặp để cho phép Email giống nhau
      // if (existingUsers.Any(u => u.Email == user.Email))
      // {
      //     throw new Exception("Email đã tồn tại!");
      // }
      // Kiểm tra UserTypeId hợp lệ
      if (user.UserTypeId <= 0)
      {
        throw new Exception("Loại người dùng không hợp lệ!");
      }
      var userType = await _context.UserTypes.FindAsync(user.UserTypeId);
      if (userType == null)
      {
        throw new Exception("Loại người dùng không tồn tại!");
      }
      // Băm mật khẩu
      user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
      // Đặt UserType thành null để tránh lỗi validation
      user.UserType = null;
      await _userRepository.AddAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
      var existingUser = await _userRepository.GetByIdAsync(user.Id);
      if (existingUser == null)
      {
        throw new Exception("Người dùng không tồn tại!");
      }

      // Kiểm tra Username trùng lặp
      var users = await _userRepository.GetAllAsync();
      if (users.Any(u => u.Username == user.Username && u.Id != user.Id))
      {
        throw new Exception("Tên đăng nhập đã tồn tại!");
      }
      var userType = await _context.UserTypes.FindAsync(user.UserTypeId);
      if (userType == null)
      {
        throw new Exception("Loại người dùng không tồn tại!");
      }

      existingUser.FullName = user.FullName;
      existingUser.Username = user.Username;
      if (!string.IsNullOrEmpty(user.Password))
      {
        existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
      }
      existingUser.Phone = user.Phone;
      existingUser.Address = user.Address;
      existingUser.Sex = user.Sex;
      existingUser.Email = user.Email;
      existingUser.UserTypeId = user.UserTypeId;

      await _userRepository.UpdateAsync(existingUser);
    }

    public async Task DeleteUserAsync(int id)
    {
      var user = await _userRepository.GetByIdAsync(id);
      if (user == null)
      {
        throw new Exception("Người dùng không tồn tại!");
      }
      await _userRepository.DeleteAsync(id);
    }
  }
}
