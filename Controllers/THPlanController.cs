using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
using PLANMHE.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace PLANMHE.Controllers
{
  [Authorize]
  public class THPlanController : Controller
  {
    private readonly ITHPlanService _thPlanService;
    private readonly IAuthService _authService;

    public THPlanController(ITHPlanService thPlanService, IAuthService authService)
    {
      _thPlanService = thPlanService;
      _authService = authService;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListTHPlan(string search = "", int pageNumber = 1)
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (!int.TryParse(userIdClaim, out int userId))
      {
        return RedirectToAction("LoginBasic", "Auth", new { returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString });
      }

      var user = await _authService.GetUserByIdAsync(userId);
      bool isAdmin = user?.UserTypeId == 1;

      // LẤY DANH SÁCH PLAN (giống HistoryPlan: KHÔNG dùng AsQueryable + ToList sớm)
      var allPlans = _thPlanService.GetPlansByUserId(userId, isAdmin);

      // LỌC THEO TÊN (server-side, để phân trang đúng)
      if (!string.IsNullOrEmpty(search))
      {
        allPlans = allPlans.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();
      }

      // PHÂN TRANG
      const int pageSize = 10;
      var totalItems = allPlans.Count;
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
      pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

      var paginatedPlans = allPlans
          .OrderByDescending(p => p.StartDate)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      // GÁN ViewBag CHUẨN int, KHÔNG string
      ViewBag.CurrentPage = pageNumber;
      ViewBag.TotalPages = totalPages;
      ViewBag.Search = search;

      // TRUYỀN IEnumerable<Plan> (giống HistoryPlan)
      return View(paginatedPlans.AsEnumerable());
    }

    public async Task<IActionResult> Detail(int id)
    {
      try
      {
        var plan = await _thPlanService.GetPlanById(id);
        if (plan == null)
        {
          return NotFound();
        }
        var assignedUsers = await _thPlanService.GetAssignedUsersByPlanId(id);
        ViewBag.AssignedUsers = assignedUsers;
        var planCells = await _thPlanService.GetPlanCellsByPlanId(id);
        bool hasLockedCells = planCells.Any(pc => pc.IsLocked && !pc.IsDeleted && !pc.IsHidden);
        if (!hasLockedCells && plan.Status != "Completed")
        {
          return RedirectToAction("ListTHPlan");
        }
        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
          }
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
          {
            validColumnIndices.Add(cell.ColumnId - 1);
          }
        }
        validColumnIndices = validColumnIndices.OrderBy(x => x).Distinct().ToList();
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        var tableData = new List<List<object>>();
        var cellFormats = new List<Dictionary<string, string>>();
        var mergedCells = new List<Dictionary<string, object>>();
        var rowHeights = new List<double>();
        var colWidths = new List<double>();
        var lockedCells = new List<Dictionary<string, object>>();
        if (maxRow > 0 && maxCol > 0)
        {
          for (int row = 1; row <= maxRow; row++)
          {
            var rowData = new List<object>();
            var rowLocked = new Dictionary<string, object>();
            for (int col = 1; col <= maxCol; col++)
            {
              var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);
              rowData.Add(cell?.Name?.Trim() ?? "");
              rowLocked[$"col{col}"] = cell?.IsLocked ?? false;
            }
            tableData.Add(rowData);
            lockedCells.Add(rowLocked);
          }
          if (totalColumnIndex != -1 && plan.Status != "Completed")
          {
            for (int row = 2; row <= maxRow; row++)
            {
              double total = 0;
              foreach (int colIndex in validColumnIndices)
              {
                var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == colIndex + 1 && !pc.IsDeleted && !pc.IsHidden);
                if (cell != null && double.TryParse(cell.Name?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value))
                {
                  total += value;
                }
              }
              tableData[row - 1][totalColumnIndex] = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##");
              var totalCell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == totalColumnIndex + 1 && !pc.IsDeleted && !pc.IsHidden);
              if (totalCell != null)
              {
                totalCell.Name = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##");
                await _thPlanService.UpdatePlanCellAsync(totalCell);
              }
              else
              {
                var newTotalCell = new PlanCell
                {
                  PlanId = id,
                  RowId = row,
                  ColumnId = totalColumnIndex + 1,
                  Name = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##"),
                  BackgroundColor = "f0f0f0",
                  FontColor = "000000",
                  FontSize = "14px",
                  FontWeight = "normal",
                  TextAlign = "center",
                  FontFamily = "Segoe UI",
                  Rowspan = 1,
                  Colspan = 1,
                  RowHeight = 30,
                  ColWidth = 60,
                  InputSettings = "",
                  IsHidden = false,
                  IsFileUpload = false,
                  IsDeleted = false,
                  IsLocked = true
                };
                await _thPlanService.UpdatePlanCellAsync(newTotalCell);
                planCells.Add(newTotalCell);
              }
            }
          }
          for (int row = 1; row <= maxRow; row++)
          {
            var rowFormats = new Dictionary<string, string>();
            for (int col = 1; col <= maxCol; col++)
            {
              var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);
              var css = new List<string>
                            {
                                $"background-color: #{cell?.BackgroundColor ?? (col - 1 == totalColumnIndex ? "f0f0f0" : "ffffff")}",
                                $"color: #{cell?.FontColor ?? "000000"}",
                                $"font-size: {cell?.FontSize ?? "14px"}",
                                $"font-weight: {cell?.FontWeight ?? "normal"}",
                                $"text-align: {cell?.TextAlign ?? (cell?.Rowspan > 1 || cell?.Colspan > 1 ? "center" : (col - 1 == totalColumnIndex ? "center" : "left"))}",
                                $"font-family: {cell?.FontFamily ?? "Segoe UI"}"
                            };
              if (col - 1 == totalColumnIndex || (cell != null && cell.IsLocked))
              {
                css.Add("cursor: not-allowed");
              }
              rowFormats[$"col{col}"] = string.Join("; ", css);
            }
            cellFormats.Add(rowFormats);
          }
          foreach (var cell in planCells.Where(pc => (pc.Rowspan > 1 || pc.Colspan > 1) && !pc.IsDeleted && !pc.IsHidden))
          {
            mergedCells.Add(new Dictionary<string, object>
                        {
                            { "startRow", cell.RowId },
                            { "startCol", cell.ColumnId },
                            { "rowSpan", cell.Rowspan ?? 1 },
                            { "colSpan", cell.Colspan ?? 1 }
                        });
          }
          foreach (var cell in planCells.Where(pc => pc.IsLocked && !pc.IsDeleted && !pc.IsHidden))
          {
            lockedCells.Add(new Dictionary<string, object>
                        {
                            { "row", cell.RowId },
                            { "col", cell.ColumnId }
                        });
          }
          rowHeights.AddRange(planCells.GroupBy(pc => pc.RowId).Select(g => g.First().RowHeight > 0 ? g.First().RowHeight : 30));
          colWidths.AddRange(planCells.GroupBy(pc => pc.ColumnId).Select(g => g.First().ColWidth > 0 ? g.First().ColWidth : 60));
        }
        ViewBag.TableData = JsonSerializer.Serialize(tableData);
        ViewBag.Formats = JsonSerializer.Serialize(cellFormats);
        ViewBag.MergedCells = JsonSerializer.Serialize(mergedCells);
        ViewBag.RowHeights = JsonSerializer.Serialize(rowHeights);
        ViewBag.ColWidths = JsonSerializer.Serialize(colWidths);
        ViewBag.TotalColumnIndex = totalColumnIndex;
        ViewBag.ValidColumnIndices = JsonSerializer.Serialize(validColumnIndices);
        ViewBag.LockedCells = JsonSerializer.Serialize(lockedCells);
        return View("~/Views/THPlan/Detail.cshtml", plan);
      }
      catch (Exception ex)
      {
        return StatusCode(500, "Lỗi khi lấy chi tiết kế hoạch: " + ex.Message);
      }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCell([FromBody] PlanCell planCell)
    {
      try
      {
        if (planCell == null || planCell.PlanId <= 0 || planCell.RowId <= 0 || planCell.ColumnId <= 0)
        {
          return Json(new { success = false, message = "Dữ liệu ô không hợp lệ." });
        }
        var existingCell = await _thPlanService.GetPlanCellsByPlanId(planCell.PlanId);
        var targetCell = existingCell.FirstOrDefault(pc => pc.RowId == planCell.RowId && pc.ColumnId == planCell.ColumnId && !pc.IsDeleted && !pc.IsHidden);
        if (targetCell != null && targetCell.IsLocked)
        {
          return Json(new { success = false, message = "Ô này đã bị khóa và không thể chỉnh sửa." });
        }
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planCell.PlanId);
        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
          }
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
          {
            validColumnIndices.Add(cell.ColumnId - 1);
          }
        }
        validColumnIndices = validColumnIndices.OrderBy(x => x).Distinct().ToList();
        await _thPlanService.UpdatePlanCellAsync(planCell);
        if (totalColumnIndex != -1 && validColumnIndices.Contains(planCell.ColumnId - 1))
        {
          var cellsToUpdate = new List<PlanCell>();
          double total = 0;
          foreach (int colIndex in validColumnIndices)
          {
            var cell = planCells.FirstOrDefault(pc => pc.RowId == planCell.RowId && pc.ColumnId == colIndex + 1 && !pc.IsDeleted && !pc.IsHidden);
            if (cell != null && double.TryParse(cell.Name?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value))
            {
              total += value;
            }
            if (cell == null && colIndex == planCell.ColumnId - 1 && double.TryParse(planCell.Name?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double newValue))
            {
              total += newValue;
            }
          }
          var totalCell = planCells.FirstOrDefault(pc => pc.RowId == planCell.RowId && pc.ColumnId == totalColumnIndex + 1 && !pc.IsDeleted && !pc.IsHidden);
          if (totalCell != null)
          {
            totalCell.Name = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##");
            cellsToUpdate.Add(totalCell);
          }
          else
          {
            var newTotalCell = new PlanCell
            {
              PlanId = planCell.PlanId,
              RowId = planCell.RowId,
              ColumnId = totalColumnIndex + 1,
              Name = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##"),
              BackgroundColor = "f0f0f0",
              FontColor = "000000",
              FontSize = "14px",
              FontWeight = "normal",
              TextAlign = "center",
              FontFamily = "Segoe UI",
              Rowspan = 1,
              Colspan = 1,
              RowHeight = planCells.FirstOrDefault(pc => pc.RowId == planCell.RowId)?.RowHeight ?? 30,
              ColWidth = planCells.FirstOrDefault(pc => pc.ColumnId == totalColumnIndex + 1)?.ColWidth ?? 60,
              InputSettings = "",
              IsHidden = false,
              IsFileUpload = false,
              IsDeleted = false,
              IsLocked = true
            };
            cellsToUpdate.Add(newTotalCell);
          }
          foreach (var cell in cellsToUpdate)
          {
            await _thPlanService.UpdatePlanCellAsync(cell);
          }
        }
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        await UpdateRowLockStatus(planCell.PlanId, planCells, totalColumnIndex, validColumnIndices, maxRow);
        planCells = await _thPlanService.GetPlanCellsByPlanId(planCell.PlanId);
        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) = PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol);
        return Json(new
        {
          success = true,
          message = "Cập nhật ô thành công.",
          data = new
          {
            tableData,
            formats = cellFormats,
            mergedCells,
            rowHeights,
            colWidths,
            totalColumnIndex,
            validColumnIndices,
            lockedCells
          }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi cập nhật ô: {ex.Message}. Inner Exception: {ex.InnerException?.Message}" });
      }
    }

    [HttpPost]
    public async Task<IActionResult> AddRow(int planId)
    {
      try
      {
        await _thPlanService.AddRowAsync(planId);
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
          }
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
          {
            validColumnIndices.Add(cell.ColumnId - 1);
          }
        }
        validColumnIndices = validColumnIndices.OrderBy(x => x).Distinct().ToList();
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }
        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) = PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol);
        return Json(new
        {
          success = true,
          message = "Thêm dòng thành công.",
          data = new
          {
            tableData,
            formats = cellFormats,
            mergedCells,
            rowHeights,
            colWidths,
            totalColumnIndex,
            validColumnIndices,
            lockedCells
          }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi thêm dòng: {ex.Message}" });
      }
    }

    [HttpPost]
    public async Task<IActionResult> AddColumn(int planId)
    {
      try
      {
        await _thPlanService.AddColumnAsync(planId);
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
          }
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
          {
            validColumnIndices.Add(cell.ColumnId - 1);
          }
        }
        validColumnIndices = validColumnIndices.OrderBy(x => x).Distinct().ToList();
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }
        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) = PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol);
        return Json(new
        {
          success = true,
          message = "Thêm cột thành công.",
          data = new
          {
            tableData,
            formats = cellFormats,
            mergedCells,
            rowHeights,
            colWidths,
            totalColumnIndex,
            validColumnIndices,
            lockedCells
          }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi thêm cột: {ex.Message}" });
      }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRow(int planId, int rowId)
    {
      try
      {
        await _thPlanService.DeleteRowAsync(planId, rowId);
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
          }
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
          {
            validColumnIndices.Add(cell.ColumnId - 1);
          }
        }
        validColumnIndices = validColumnIndices.OrderBy(x => x).Distinct().ToList();
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }
        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) = PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol);
        return Json(new
        {
          success = true,
          message = "Xóa dòng thành công.",
          data = new
          {
            tableData,
            formats = cellFormats,
            mergedCells,
            rowHeights,
            colWidths,
            totalColumnIndex,
            validColumnIndices,
            lockedCells
          }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi xóa dòng: {ex.Message}" });
      }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteColumn(int planId, int columnId)
    {
      try
      {
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        var totalColumn = planCells.FirstOrDefault(pc => pc.RowId == 1 && (pc.Name.ToLower().Contains("tổng cộng") || pc.Name.ToLower().Contains("total")));
        if (totalColumn != null && totalColumn.ColumnId == columnId)
        {
          return Json(new { success = false, message = "Không thể xóa cột Tổng cộng." });
        }
        await _thPlanService.DeleteColumnAsync(planId, columnId);
        planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
          }
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
          {
            validColumnIndices.Add(cell.ColumnId - 1);
          }
        }
        validColumnIndices = validColumnIndices.OrderBy(x => x).Distinct().ToList();
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }
        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) = PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol);
        return Json(new
        {
          success = true,
          message = "Xóa cột thành công.",
          data = new
          {
            tableData,
            formats = cellFormats,
            mergedCells,
            rowHeights,
            colWidths,
            totalColumnIndex,
            validColumnIndices,
            lockedCells
          }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi xóa cột: {ex.Message}" });
      }
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmPlan(int planId)
    {
      try
      {
        var plan = await _thPlanService.GetPlanById(planId);
        if (plan == null)
        {
          return Json(new { success = false, message = "Không tìm thấy kế hoạch!" });
        }
        if (plan.Status == "Completed")
        {
          return Json(new { success = false, message = "Kế hoạch đã hoàn thành rồi!" });
        }
        await _thPlanService.ConfirmPlanAsync(planId);
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
          }
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
          {
            validColumnIndices.Add(cell.ColumnId - 1);
          }
        }
        validColumnIndices = validColumnIndices.OrderBy(x => x).Distinct().ToList();
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) = PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol);
        return Json(new
        {
          success = true,
          message = "✅ KẾ HOẠCH ĐÃ HOÀN THÀNH!",
          data = new
          {
            tableData,
            formats = cellFormats,
            mergedCells,
            rowHeights,
            colWidths,
            totalColumnIndex,
            validColumnIndices,
            lockedCells
          }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = ex.Message });
      }
    }

    private async Task UpdateRowLockStatus(int planId, List<PlanCell> planCells, int totalColumnIndex, List<int> validColumnIndices, int maxRow)
    {
      var cellsToUpdate = new List<PlanCell>();
      for (int row = 2; row <= maxRow; row++)
      {
        var currentTotalCell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == totalColumnIndex + 1 && !pc.IsDeleted && !pc.IsHidden);
        var aboveTotalCell = planCells.FirstOrDefault(pc => pc.RowId == row - 1 && pc.ColumnId == totalColumnIndex + 1 && !pc.IsDeleted && !pc.IsHidden);
        double currentTotal = currentTotalCell != null && double.TryParse(currentTotalCell.Name?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double c) ? c : 0;
        double aboveTotal = aboveTotalCell != null && double.TryParse(aboveTotalCell.Name?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double a) ? a : 0;
        var sampleDataCell = planCells.FirstOrDefault(pc => pc.RowId == row && validColumnIndices.Contains(pc.ColumnId - 1) && !pc.IsDeleted && !pc.IsHidden);
        bool isCurrentOpen = sampleDataCell != null && !sampleDataCell.IsLocked;
        if (isCurrentOpen && currentTotal >= aboveTotal && aboveTotalCell != null)
        {
          var rowCells = planCells.Where(pc => pc.RowId == row && !pc.IsDeleted && !pc.IsHidden).ToList();
          foreach (var cell in rowCells)
          {
            cell.IsLocked = true;
            cellsToUpdate.Add(cell);
          }
          int nextRowToOpen = row + 2;
          if (nextRowToOpen <= maxRow)
          {
            var nextRowCells = planCells.Where(pc => pc.RowId == nextRowToOpen && !pc.IsDeleted && !pc.IsHidden && pc.ColumnId != totalColumnIndex + 1).ToList();
            foreach (var cell in nextRowCells)
            {
              cell.IsLocked = false;
              cellsToUpdate.Add(cell);
            }
          }
        }
      }
      foreach (var cell in cellsToUpdate)
      {
        await _thPlanService.UpdatePlanCellAsync(cell);
      }
    }

    private (List<List<object>> tableData, List<Dictionary<string, string>> cellFormats, List<Dictionary<string, object>> mergedCells, List<double> rowHeights, List<double> colWidths, List<Dictionary<string, object>> lockedCells) PrepareTableData(List<PlanCell> planCells, int totalColumnIndex, int maxRow, int maxCol)
    {
      var tableData = new List<List<object>>();
      var cellFormats = new List<Dictionary<string, string>>();
      var mergedCells = new List<Dictionary<string, object>>();
      var rowHeights = new List<double>();
      var colWidths = new List<double>();
      var lockedCells = new List<Dictionary<string, object>>();
      if (maxRow > 0 && maxCol > 0)
      {
        for (int row = 1; row <= maxRow; row++)
        {
          var rowData = new List<object>();
          for (int col = 1; col <= maxCol; col++)
          {
            var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);
            rowData.Add(cell?.Name?.Trim() ?? (col == totalColumnIndex + 1 ? "0" : ""));
          }
          tableData.Add(rowData);
        }
        for (int row = 1; row <= maxRow; row++)
        {
          var rowFormats = new Dictionary<string, string>();
          for (int col = 1; col <= maxCol; col++)
          {
            var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);
            var css = new List<string>
                        {
                            $"background-color: #{cell?.BackgroundColor ?? (col - 1 == totalColumnIndex ? "f0f0f0" : "ffffff")}",
                            $"color: #{cell?.FontColor ?? "000000"}",
                            $"font-size: {cell?.FontSize ?? "14px"}",
                            $"font-weight: {cell?.FontWeight ?? "normal"}",
                            $"text-align: {cell?.TextAlign ?? (cell?.Rowspan > 1 || cell?.Colspan > 1 ? "center" : (col - 1 == totalColumnIndex ? "center" : "left"))}",
                            $"font-family: {cell?.FontFamily ?? "Segoe UI"}"
                        };
            if (col - 1 == totalColumnIndex || (cell != null && cell.IsLocked))
            {
              css.Add("cursor: not-allowed");
            }
            rowFormats[$"col{col}"] = string.Join("; ", css);
          }
          cellFormats.Add(rowFormats);
        }
        foreach (var cell in planCells.Where(pc => (pc.Rowspan > 1 || pc.Colspan > 1) && !pc.IsDeleted && !pc.IsHidden))
        {
          mergedCells.Add(new Dictionary<string, object>
                    {
                        { "startRow", cell.RowId },
                        { "startCol", cell.ColumnId },
                        { "rowSpan", cell.Rowspan ?? 1 },
                        { "colSpan", cell.Colspan ?? 1 }
                    });
        }
        foreach (var cell in planCells.Where(pc => pc.IsLocked && !pc.IsDeleted && !pc.IsHidden))
        {
          lockedCells.Add(new Dictionary<string, object>
                    {
                        { "row", cell.RowId },
                        { "col", cell.ColumnId }
                    });
        }
        rowHeights.AddRange(planCells.GroupBy(pc => pc.RowId).Select(g => g.First().RowHeight > 0 ? g.First().RowHeight : 30));
        colWidths.AddRange(planCells.GroupBy(pc => pc.ColumnId).Select(g => g.First().ColWidth > 0 ? g.First().ColWidth : 60));
      }
      return (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells);
    }
  }
}
