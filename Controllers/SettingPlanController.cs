using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PLANMHE.Models;
using PLANMHE.Service;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

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
        // Parse JSON
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

        // Lấy Description (không bắt buộc)
        string description = request.TryGetProperty("Description", out var descElement) ? descElement.GetString() : null;

        // Tạo object Plan
        var plan = new Plan
        {
          Name = nameElement.GetString(),
          Description = description,
          StartDate = startDate,
          EndDate = endDate
        };

        // Parse SelectedUserIds
        var selectedUserIds = new List<int>();
        foreach (var idElement in userIdsElement.EnumerateArray())
        {
          if (idElement.TryGetInt32(out int id))
          {
            selectedUserIds.Add(id);
          }
        }

        // Lưu kế hoạch
        await _kehoachService.AddPlanAsync(plan, selectedUserIds);
        return Json(new { success = true, message = "Lưu kế hoạch thành công." });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Lỗi khi lưu kế hoạch: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        return Json(new { success = false, message = "Lỗi khi lưu kế hoạch: " + ex.Message });
      }
    }

    public IActionResult Listkehoach()
    {
      return View();
    }
  }
}
