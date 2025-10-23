using Microsoft.AspNetCore.Mvc;
using PLANMHE.Models;
using PLANMHE.Service;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PLANMHE.Controllers
{
  public class SettingPlanController : Controller
  {
    private readonly IKehoachService _kehoachService;

    public SettingPlanController(IKehoachService kehoachService)
    {
      _kehoachService = kehoachService;
    }

    public async Task<IActionResult> Create()
    {
      var users = await _kehoachService.GetAllUsersAsync();
      ViewBag.Users = new SelectList(users, "Id", "FullName");
      return View(new Plan());
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlan([FromBody] JsonElement request)
    {
      try
      {
        if (!request.TryGetProperty("Name", out var nameElement) || string.IsNullOrEmpty(nameElement.GetString()))
        {
          return Json(new { success = false, message = "Tên kế hoạch là bắt buộc." });
        }
        if (!request.TryGetProperty("SelectedUserIds", out var userIdsElement) || userIdsElement.GetArrayLength() == 0)
        {
          return Json(new { success = false, message = "Vui lòng chọn ít nhất một người thực hiện." });
        }
        if (!request.TryGetProperty("StartDate", out var startDateElement) || !DateTime.TryParse(startDateElement.GetString(), out var startDate))
        {
          return Json(new { success = false, message = "Thời gian bắt đầu không hợp lệ." });
        }
        if (!request.TryGetProperty("EndDate", out var endDateElement) || !DateTime.TryParse(endDateElement.GetString(), out var endDate))
        {
          return Json(new { success = false, message = "Thời gian kết thúc không hợp lệ." });
        }
        string description = request.TryGetProperty("Description", out var descElement) ? descElement.GetString() : null;
        var plan = new Plan
        {
          Name = nameElement.GetString(),
          Description = description,
          StartDate = startDate,
          EndDate = endDate
        };
        var selectedUserIds = new List<int>();
        foreach (var userIdElement in userIdsElement.EnumerateArray())
        {
          if (userIdElement.TryGetInt32(out int userId))
          {
            selectedUserIds.Add(userId);
          }
        }
        int planId = await _kehoachService.AddPlanAsync(plan, selectedUserIds);
        return Json(new { success = true, message = "Lưu kế hoạch thành công.", redirectUrl = Url.Action("Detail", "DetailSettingPlan", new { id = planId }) });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Lỗi khi lưu kế hoạch: " + ex.Message });
      }
    }
    public async Task<IActionResult> Listkehoach(int pageNumber = 1)
    {
      const int pageSize = 10;
      var allPlans = await _kehoachService.GetAllPlansAsync();
      var activePlans = allPlans.Where(p => p.Status == "Active").ToList();
      var plans = activePlans
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();
      ViewBag.TotalPages = (int)Math.Ceiling(activePlans.Count() / (double)pageSize);
      ViewBag.CurrentPage = pageNumber;
      return View(plans);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePlan(int planId)
    {
      try
      {
        await _kehoachService.DeletePlanAsync(planId);
        return Json(new { success = true, message = "Xóa kế hoạch thành công." });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Lỗi khi xóa kế hoạch: " + ex.Message });
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetAssignedUsers(int planId)
    {
      try
      {
        var userIds = await _kehoachService.GetPlanUsersAsync(planId);
        var userList = await _kehoachService.GetAllUsersAsync();
        var assignedUsers = userList
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { id = u.Id, fullName = u.FullName })
            .ToList();
        return Json(new { success = true, users = assignedUsers });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Lỗi khi lấy danh sách người dùng đã gán: " + ex.Message });
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
      try
      {
        var users = await _kehoachService.GetAllUsersAsync();
        var userList = users.Select(u => new { id = u.Id, fullName = u.FullName }).ToList();
        return Json(userList);
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Lỗi khi lấy danh sách người dùng: " + ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePlan([FromBody] JsonElement request)
    {
      try
      {
        if (!request.TryGetProperty("id", out var idElement) || !idElement.TryGetInt32(out int id))
        {
          return Json(new { success = false, message = "ID kế hoạch không hợp lệ." });
        }
        if (!request.TryGetProperty("name", out var nameElement) || string.IsNullOrEmpty(nameElement.GetString()))
        {
          return Json(new { success = false, message = "Tên kế hoạch là bắt buộc." });
        }
        if (!request.TryGetProperty("selectedUserIds", out var userIdsElement) || userIdsElement.GetArrayLength() == 0)
        {
          return Json(new { success = false, message = "Vui lòng chọn ít nhất một người thực hiện." });
        }
        if (!request.TryGetProperty("startDate", out var startDateElement) || !DateTime.TryParse(startDateElement.GetString(), out var startDate))
        {
          return Json(new { success = false, message = "Thời gian bắt đầu không hợp lệ." });
        }
        if (!request.TryGetProperty("endDate", out var endDateElement) || !DateTime.TryParse(endDateElement.GetString(), out var endDate))
        {
          return Json(new { success = false, message = "Thời gian kết thúc không hợp lệ." });
        }
        string description = request.TryGetProperty("description", out var descElement) ? descElement.GetString() : null;
        string status = request.TryGetProperty("status", out var statusElement) ? statusElement.GetString() : "Active"; // Mặc định là Active
        var plan = new Plan
        {
          Id = id,
          Name = nameElement.GetString(),
          Description = description,
          StartDate = startDate,
          EndDate = endDate,
          Status = status
        };
        var selectedUserIds = new List<int>();
        foreach (var userIdElement in userIdsElement.EnumerateArray())
        {
          if (userIdElement.TryGetInt32(out int userId))
          {
            selectedUserIds.Add(userId);
          }
        }
        await _kehoachService.UpdatePlanAsync(plan, selectedUserIds);
        return Json(new { success = true, message = "Cập nhật kế hoạch thành công." });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Lỗi khi cập nhật kế hoạch: " + ex.Message });
      }
    }
  }
}
