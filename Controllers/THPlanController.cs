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

    public IActionResult Index() => View();

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

      var allPlans = _thPlanService.GetPlansByUserId(userId, isAdmin);

      if (!string.IsNullOrEmpty(search))
      {
        allPlans = allPlans.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();
      }

      const int pageSize = 10;
      var totalItems = allPlans.Count;
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
      pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

      var paginatedPlans = allPlans
          .OrderByDescending(p => p.StartDate)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      ViewBag.CurrentPage = pageNumber;
      ViewBag.TotalPages = totalPages;
      ViewBag.Search = search;

      return View(paginatedPlans.AsEnumerable());
    }

    public async Task<IActionResult> Detail(int id)
    {
      try
      {
        var plan = await _thPlanService.GetPlanById(id);
        if (plan == null) return NotFound();

        var assignedUsers = await _thPlanService.GetAssignedUsersByPlanId(id);
        ViewBag.AssignedUsers = assignedUsers;

        var planCells = await _thPlanService.GetPlanCellsByPlanId(id);

        // === XÓA HOÀN TOÀN ĐOẠN KIỂM TRA LOCK (KHÔNG CẦN KHI MỞ TRANG) ===
        // bool hasLockedCells = planCells.Any(pc => pc.IsLocked && !pc.IsDeleted && !pc.IsHidden);
        // if (!hasLockedCells && plan.Status != "Completed")
        //     return RedirectToAction("ListTHPlan");

        // === DÙNG TRỰC TIẾP STATUS (NHANH HƠN) ===
        if (plan.Status != "Completed" && !planCells.Any(pc => pc.IsLocked))
          return RedirectToAction("ListTHPlan");

        int totalColumnIndex = -1;
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;
        }

        var validColumnIndices = headerCells
            .Where(c => validColumns.Contains(c.Name.Trim()))
            .Select(c => c.ColumnId - 1)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

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
          // === XÓA HOÀN TOÀN ĐOẠN TÍNH TỔNG TRƯỚC (ĐÃ TÍNH KHI NHẬP) ===
          // if (totalColumnIndex != -1 && plan.Status != "Completed") { ... }

          // === CHỈ GỌI PrepareTableData 1 LẦN DUY NHẤT ===
          var (td, cf, mc, rh, cw, lc) = PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol, validColumnIndices);

          tableData = td;
          cellFormats = cf;
          mergedCells = mc;
          rowHeights = rh;
          colWidths = cw;
          lockedCells = lc;
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
          return Json(new { success = false, message = "Dữ liệu ô không hợp lệ." });

        // LẤY DỮ LIỆU 1 LẦN DUY NHẤT
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planCell.PlanId);
        var targetCell = planCells.FirstOrDefault(pc =>
            pc.RowId == planCell.RowId && pc.ColumnId == planCell.ColumnId && !pc.IsDeleted && !pc.IsHidden);
        if (targetCell != null && targetCell.IsLocked)
          return Json(new { success = false, message = "Ô này đã bị khóa và không thể chỉnh sửa." });

        // XÁC ĐỊNH CỘT TỔNG & CÁC CỘT NGÀY
        int totalColumnIndex = -1;
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;

        var validColumnIndices = headerCells
            .Where(c => validColumns.Contains(c.Name.Trim()))
            .Select(c => c.ColumnId - 1)
            .Distinct()
            .ToList();

        // CẬP NHẬT Ô NGƯỜI DÙNG
        await _thPlanService.UpdatePlanCellAsync(planCell);

        // TÍNH TỔNG DÙNG LOOKUP (SIÊU NHANH)
        if (totalColumnIndex != -1 && validColumnIndices.Contains(planCell.ColumnId - 1))
        {
          var cellLookup = planCells.ToLookup(pc => (pc.RowId, pc.ColumnId));
          double total = 0;
          foreach (int colIdx in validColumnIndices)
          {
            var cell = cellLookup[(planCell.RowId, colIdx + 1)].FirstOrDefault();
            if (cell != null && double.TryParse(cell.Name?.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double val))
              total += val;
          }

          string totalText = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##");
          var totalCell = cellLookup[(planCell.RowId, totalColumnIndex + 1)].FirstOrDefault()
                         ?? new PlanCell { PlanId = planCell.PlanId, RowId = planCell.RowId, ColumnId = totalColumnIndex + 1 };

          totalCell.Name = totalText;
          totalCell.BackgroundColor = "f0f0f0";
          totalCell.IsLocked = true;
          totalCell.TextAlign = "center";
          totalCell.FontFamily = "Segoe UI";

          await _thPlanService.UpdatePlanCellAsync(totalCell);
        }

        // CẬP NHẬT LOCK & TẢI LẠI DỮ LIỆU
        int maxRow = planCells.Max(pc => pc.RowId);
        if (totalColumnIndex != -1 && validColumnIndices.Any())
          await UpdateRowLockStatus(planCell.PlanId, planCells, totalColumnIndex, validColumnIndices, maxRow);

        // TẢI LẠI DỮ LIỆU MỚI NHẤT
        planCells = await _thPlanService.GetPlanCellsByPlanId(planCell.PlanId);
        int maxCol = planCells.Max(pc => pc.ColumnId);

        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) =
            PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol, validColumnIndices);

        return Json(new
        {
          success = true,
          message = "Cập nhật thành công.",
          data = new { tableData, formats = cellFormats, mergedCells, rowHeights, colWidths, totalColumnIndex, validColumnIndices, lockedCells }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
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
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();

        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;
        }

        var validColumnIndices = headerCells
            .Where(c => validColumns.Contains(c.Name.Trim()))
            .Select(c => c.ColumnId - 1)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;

        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }

        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) =
            PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol, validColumnIndices);

        return Json(new
        {
          success = true,
          message = "Thêm dòng thành công.",
          data = new { tableData, formats = cellFormats, mergedCells, rowHeights, colWidths, totalColumnIndex, validColumnIndices, lockedCells }
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
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();

        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;
        }

        var validColumnIndices = headerCells
            .Where(c => validColumns.Contains(c.Name.Trim()))
            .Select(c => c.ColumnId - 1)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;

        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }

        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) =
            PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol, validColumnIndices);

        return Json(new
        {
          success = true,
          message = "Thêm cột thành công.",
          data = new { tableData, formats = cellFormats, mergedCells, rowHeights, colWidths, totalColumnIndex, validColumnIndices, lockedCells }
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
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();

        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;
        }

        var validColumnIndices = headerCells
            .Where(c => validColumns.Contains(c.Name.Trim()))
            .Select(c => c.ColumnId - 1)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;

        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }

        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) =
            PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol, validColumnIndices);

        return Json(new
        {
          success = true,
          message = "Xóa dòng thành công.",
          data = new { tableData, formats = cellFormats, mergedCells, rowHeights, colWidths, totalColumnIndex, validColumnIndices, lockedCells }
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
          return Json(new { success = false, message = "Không thể xóa cột Tổng cộng." });

        await _thPlanService.DeleteColumnAsync(planId, columnId);
        planCells = await _thPlanService.GetPlanCellsByPlanId(planId);

        int totalColumnIndex = -1;
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();

        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;
        }

        var validColumnIndices = headerCells
            .Where(c => validColumns.Contains(c.Name.Trim()))
            .Select(c => c.ColumnId - 1)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;

        if (totalColumnIndex != -1 && validColumnIndices.Any())
        {
          await UpdateRowLockStatus(planId, planCells, totalColumnIndex, validColumnIndices, maxRow);
          planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        }

        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) =
            PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol, validColumnIndices);

        return Json(new
        {
          success = true,
          message = "Xóa cột thành công.",
          data = new { tableData, formats = cellFormats, mergedCells, rowHeights, colWidths, totalColumnIndex, validColumnIndices, lockedCells }
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
        if (plan == null) return Json(new { success = false, message = "Không tìm thấy kế hoạch!" });
        if (plan.Status == "Completed") return Json(new { success = false, message = "Kế hoạch đã hoàn thành rồi!" });

        await _thPlanService.ConfirmPlanAsync(planId);
        var planCells = await _thPlanService.GetPlanCellsByPlanId(planId);

        int totalColumnIndex = -1;
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();

        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;
        }

        var validColumnIndices = headerCells
            .Where(c => validColumns.Contains(c.Name.Trim()))
            .Select(c => c.ColumnId - 1)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;

        var (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells) =
            PrepareTableData(planCells, totalColumnIndex, maxRow, maxCol, validColumnIndices);

        return Json(new
        {
          success = true,
          message = "KẾ HOẠCH ĐÃ HOÀN THÀNH!",
          data = new { tableData, formats = cellFormats, mergedCells, rowHeights, colWidths, totalColumnIndex, validColumnIndices, lockedCells }
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = ex.Message });
      }
    }

    private async Task UpdateRowLockStatus(int planId, List<PlanCell> planCells, int totalColumnIndex, List<int> validColumnIndices, int maxRow)
    {
      var cellLookup = planCells.ToLookup(pc => (pc.RowId, pc.ColumnId));
      var cellsToUpdate = new List<PlanCell>();

      for (int row = 2; row <= maxRow; row++)
      {
        var currentTotalCell = cellLookup[(row, totalColumnIndex + 1)].FirstOrDefault();
        var aboveTotalCell = cellLookup[(row - 1, totalColumnIndex + 1)].FirstOrDefault();

        double currentTotal = ParseDouble(currentTotalCell?.Name);
        double aboveTotal = ParseDouble(aboveTotalCell?.Name);

        var sampleDataCell = validColumnIndices
            .Select(colIdx => cellLookup[(row, colIdx + 1)].FirstOrDefault())
            .FirstOrDefault(c => c != null && !c.IsLocked);

        if (sampleDataCell != null && currentTotal >= aboveTotal && aboveTotalCell != null)
        {
          // Khóa toàn bộ dòng hiện tại
          foreach (var cell in planCells.Where(pc => pc.RowId == row && !pc.IsDeleted && !pc.IsHidden))
          {
            cell.IsLocked = true;
            cellsToUpdate.Add(cell);
          }

          // Mở dòng +2
          int nextRow = row + 2;
          if (nextRow <= maxRow)
          {
            foreach (var cell in planCells.Where(pc => pc.RowId == nextRow && !pc.IsDeleted && !pc.IsHidden && pc.ColumnId != totalColumnIndex + 1))
            {
              cell.IsLocked = false;
              cellsToUpdate.Add(cell);
            }
          }
        }
      }

      foreach (var cell in cellsToUpdate)
        await _thPlanService.UpdatePlanCellAsync(cell);
    }

    // Helper
    private double ParseDouble(string text) =>
        double.TryParse(text?.Replace(",", "."), System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out double val) ? val : 0;

    private (List<List<object>> tableData, List<Dictionary<string, string>> cellFormats,
         List<Dictionary<string, object>> mergedCells, List<double> rowHeights,
         List<double> colWidths, List<Dictionary<string, object>> lockedCells)
    PrepareTableData(List<PlanCell> planCells, int totalColumnIndex, int maxRow, int maxCol, List<int> validColumnIndices)
{
    var tableData = new List<List<object>>();
    var cellFormats = new List<Dictionary<string, string>>();
    var mergedCells = new List<Dictionary<string, object>>();
    var rowHeights = new List<double>();
    var colWidths = new List<double>();
    var lockedCells = new List<Dictionary<string, object>>();

    if (maxRow <= 0 || maxCol <= 0)
        return (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells);

    // 1. XÂY TABLEDATA
    for (int row = 1; row <= maxRow; row++)
    {
        var rowData = new List<object>();
        for (int col = 1; col <= maxCol; col++)
        {
            var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);
            rowData.Add(cell?.Name?.Trim() ?? (col - 1 == totalColumnIndex ? "0" : ""));
        }
        tableData.Add(rowData);
    }

    // 2. TÔ MÀU TOÀN DÒNG NẾU CÓ 1 Ô NGÀY MỞ
    for (int row = 1; row <= maxRow; row++)
    {
        var rowFormats = new Dictionary<string, string>();
        bool isRowOpen = false;

        // Kiểm tra: có ít nhất 1 ô ngày (Thứ 4 → Thứ 3) đang mở không?
        for (int col = 1; col <= maxCol; col++)
        {
            var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);
            bool isDataCell = row > 1 && validColumnIndices.Contains(col - 1);
            bool isCellOpen = cell != null && !cell.IsLocked;

            if (isDataCell && isCellOpen)
            {
                isRowOpen = true;
                break;
            }
        }

        // Áp dụng cho TẤT CẢ ô trong dòng
        for (int col = 1; col <= maxCol; col++)
        {
            var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);

            // MÀU NỀN: Dòng mở → #f0f0f0 | Cột tổng → #f0f0f0 | Còn lại → màu cell
            string bgColor = isRowOpen ? "f0f0f0" 
                             : (cell?.BackgroundColor ?? (col - 1 == totalColumnIndex ? "f0f0f0" : "ffffff"));

            var css = new List<string>
            {
                $"background-color: #{bgColor}",
                $"color: #{cell?.FontColor ?? "000000"}",
                $"font-size: {cell?.FontSize ?? "14px"}",
                $"font-weight: {cell?.FontWeight ?? "normal"}",
                $"text-align: {cell?.TextAlign ?? (cell?.Rowspan > 1 || cell?.Colspan > 1 ? "center" : (col - 1 == totalColumnIndex ? "center" : "left"))}",
                $"font-family: {cell?.FontFamily ?? "Segoe UI"}"
            };

            if (col - 1 == totalColumnIndex || (cell?.IsLocked ?? false))
                css.Add("cursor: not-allowed");

            rowFormats[$"col{col}"] = string.Join("; ", css);
        }
        cellFormats.Add(rowFormats);
    }

    // 3. MERGED + LOCKED + SIZE (giữ nguyên)
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

    rowHeights.AddRange(planCells
        .GroupBy(pc => pc.RowId)
        .OrderBy(g => g.Key)
        .Select(g => g.First().RowHeight > 0 ? g.First().RowHeight : 30));

    colWidths.AddRange(planCells
        .GroupBy(pc => pc.ColumnId)
        .OrderBy(g => g.Key)
        .Select(g => g.First().ColWidth > 0 ? g.First().ColWidth : 60));

    return (tableData, cellFormats, mergedCells, rowHeights, colWidths, lockedCells);
}
  }
}
