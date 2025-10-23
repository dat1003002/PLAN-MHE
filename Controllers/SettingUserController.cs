using Microsoft.AspNetCore.Mvc;
using PLANMHE.Models;
using PLANMHE.Services.Interfaces;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PLANMHE.Controllers
{
  public class SettingUserController : Controller
  {
    private readonly IUserTypeService _userTypeService;
    private readonly IUserService _userService;

    public SettingUserController(IUserTypeService userTypeService, IUserService userService)
    {
      _userTypeService = userTypeService;
      _userService = userService;
    }

    public async Task<IActionResult> ListRoleSetting()
    {
      var userTypes = await _userTypeService.GetAllUserTypesAsync();
      return View(userTypes);
    }

    public async Task<IActionResult> ListUser()
    {
      try
      {
        var users = await _userService.GetAllUsersAsync();
        ViewBag.UserTypes = await _userTypeService.GetAllUserTypesAsync();
        return View(users);
      }
      catch (Exception ex)
      {
        return View(new List<User>());
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
      try
      {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
          return NotFound(new { message = "Người dùng không tồn tại!" });
        }
        return Ok(user);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "Không thể lấy thông tin người dùng: " + ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserType([FromBody] UserType userType)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      try
      {
        await _userTypeService.AddUserTypeAsync(userType);
        return Ok(new { message = "Thêm loại người dùng thành công!" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "Không thể thêm loại người dùng: " + ex.Message });
      }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserType([FromBody] UserType userType)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      try
      {
        await _userTypeService.UpdateUserTypeAsync(userType);
        return Ok(new { message = "Cập nhật loại người dùng thành công!" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "Không thể cập nhật loại người dùng: " + ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUserType([FromBody] JsonElement request)
    {
      int id = 0;
      if (!request.TryGetProperty("id", out var idElement) && !request.TryGetProperty("Id", out idElement))
      {
        return BadRequest(new { message = "ID không được cung cấp trong request!" });
      }
      if (!idElement.TryGetInt32(out id) || id <= 0)
      {
        return BadRequest(new { message = "ID không hợp lệ!" });
      }
      try
      {
        await _userTypeService.DeleteUserTypeAsync(id);
        return Ok(new { message = "Xóa loại người dùng thành công!" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
        return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
      }
      try
      {
        await _userService.AddUserAsync(user);
        return Ok(new { message = "Thêm người dùng thành công!" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "Không thể thêm người dùng: " + ex.Message });
      }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
        return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
      }
      try
      {
        await _userService.UpdateUserAsync(user);
        return Ok(new { message = "Cập nhật người dùng thành công!" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "Không thể cập nhật người dùng: " + ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] JsonElement request)
    {
      int id = 0;
      if (!request.TryGetProperty("id", out var idElement) || !idElement.TryGetInt32(out id) || id <= 0)
      {
        return BadRequest(new { message = "ID không hợp lệ!" });
      }
      try
      {
        await _userService.DeleteUserAsync(id);
        return Ok(new { message = "Xóa người dùng thành công!" });
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserTypesJson()
    {
      try
      {
        var userTypes = await _userTypeService.GetAllUserTypesAsync();
        return Ok(userTypes);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "Không thể lấy danh sách loại người dùng: " + ex.Message });
      }
    }
  }
}
