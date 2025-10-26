using Microsoft.AspNetCore.Mvc;
using PLANMHE.Models;
using PLANMHE.Service;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Style;

namespace PLANMHE.Controllers
{
  public class DetailSettingPlanController : Controller
  {
    private readonly IDetailkehoachService _detailKehoachService;
    private readonly IKehoachService _kehoachService;

    public DetailSettingPlanController(IDetailkehoachService detailKehoachService, IKehoachService kehoachService)
    {
      _detailKehoachService = detailKehoachService;
      _kehoachService = kehoachService;
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
      try
      {
        var plans = await _kehoachService.GetAllPlansAsync();
        var plan = plans.FirstOrDefault(p => p.Id == id);
        if (plan == null)
        {
          return NotFound();
        }
        var userIds = await _kehoachService.GetPlanUsersAsync(id) ?? new List<int>();
        var users = await _kehoachService.GetAllUsersAsync() ?? new List<User>();
        var assignedUsers = users.Where(u => userIds.Contains(u.Id)).ToList();
        var planCells = await _detailKehoachService.GetPlanCellsAsync(id) ?? new List<PlanCell>();
        var tableData = new List<List<object>>();
        var cellFormats = new List<Dictionary<string, string>>();
        var mergedCells = new List<Dictionary<string, object>>();
        var rowHeights = new List<double>();
        var colWidths = new List<double>();
        var lockedCells = new List<Dictionary<string, bool>>();
        var validColumnIndices = new List<int>();
        int totalColumnIndex = -1;
        int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
        int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
        if (maxRow > 0 && maxCol > 0)
        {
          for (int row = 1; row <= maxRow; row++)
          {
            var rowData = new List<object>();
            var rowLocked = new Dictionary<string, bool>();
            for (int col = 1; col <= maxCol; col++)
            {
              var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col);
              rowData.Add(cell?.Name?.Trim() ?? "");
              rowLocked[$"col{col}"] = cell?.IsLocked ?? false;
            }
            tableData.Add(rowData);
            lockedCells.Add(rowLocked);
          }
          for (int row = 1; row <= maxRow; row++)
          {
            var rowFormats = new Dictionary<string, string>();
            for (int col = 1; col <= maxCol; col++)
            {
              var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col);
              var css = new List<string>
                            {
                                $"background-color: #{cell?.BackgroundColor ?? "ffffff"}",
                                $"color: #{cell?.FontColor ?? "000000"}",
                                $"font-size: {cell?.FontSize ?? "11pt"}",
                                $"font-weight: {cell?.FontWeight ?? "normal"}",
                                $"text-align: {cell?.TextAlign ?? (cell?.Rowspan > 1 || cell?.Colspan > 1 ? "center" : "left")}",
                                $"font-family: {cell?.FontFamily ?? "Arial"}"
                            };
              rowFormats[$"col{col}"] = string.Join("; ", css);
            }
            cellFormats.Add(rowFormats);
          }
          foreach (var cell in planCells.Where(pc => pc.Rowspan > 1 || pc.Colspan > 1))
          {
            mergedCells.Add(new Dictionary<string, object>
                        {
                            { "startRow", cell.RowId },
                            { "startCol", cell.ColumnId },
                            { "rowSpan", cell.Rowspan },
                            { "colSpan", cell.Colspan }
                        });
          }
          rowHeights.AddRange(planCells.GroupBy(pc => pc.RowId).Select(g => g.First().RowHeight > 0 ? g.First().RowHeight : 30));
          colWidths.AddRange(planCells.GroupBy(pc => pc.ColumnId).Select(g => g.First().ColWidth > 0 ? g.First().ColWidth : 60));
          var headerCells = planCells.Where(pc => pc.RowId == 1).ToList();
          string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
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
          if (totalColumnIndex != -1)
          {
            for (int row = 2; row <= maxRow; row++)
            {
              double total = 0;
              foreach (int colIndex in validColumnIndices)
              {
                var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == colIndex + 1);
                if (cell != null && double.TryParse(cell.Name?.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value))
                {
                  total += value;
                }
              }
              tableData[row - 1][totalColumnIndex] = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##");
            }
          }
        }
        ViewBag.AssignedUsers = assignedUsers;
        ViewBag.TableData = JsonSerializer.Serialize(tableData);
        ViewBag.Formats = JsonSerializer.Serialize(cellFormats);
        ViewBag.MergedCells = JsonSerializer.Serialize(mergedCells);
        ViewBag.RowHeights = JsonSerializer.Serialize(rowHeights);
        ViewBag.ColWidths = JsonSerializer.Serialize(colWidths);
        ViewBag.TotalColumnIndex = totalColumnIndex;
        ViewBag.ValidColumnIndices = JsonSerializer.Serialize(validColumnIndices);
        ViewBag.LockedCells = JsonSerializer.Serialize(lockedCells);
        return View("~/Views/SettingPlan/Detail.cshtml", plan);
      }
      catch (Exception ex)
      {
        return StatusCode(500, "Lỗi khi lấy chi tiết kế hoạch: " + ex.Message);
      }
    }

    [HttpPost]
    public async Task<IActionResult> UploadExcel(IFormFile file, int planId)
    {
      if (file == null || file.Length == 0)
        return Json(new { success = false, message = "Vui lòng chọn file Excel." });

      try
      {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];
        int originalRowCount = worksheet.Dimension?.Rows ?? 0;
        int colCount = worksheet.Dimension?.Columns ?? 0;

        if (originalRowCount == 0 || colCount == 0)
          return Json(new { success = false, message = "File Excel không chứa dữ liệu." });

        int lastNonEmptyRow = GetLastNonEmptyRow(worksheet, colCount);
        int rowCount = lastNonEmptyRow > 0 ? lastNonEmptyRow + 1 : 1;

        var tableData = new List<List<object>>(rowCount);
        var cellFormats = new List<Dictionary<string, string>>(rowCount);
        var mergedCells = new List<Dictionary<string, object>>();
        var rowHeights = new List<double>(rowCount);
        var colWidths = new List<double>(colCount);
        int totalColumnIndex = -1;
        List<int> validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };

        var headerRow = worksheet.Cells[1, 1, 1, colCount].Select(c => c.Text?.Trim() ?? "").ToArray();
        bool hasTotalColumn = false;
        for (int col = 0; col < colCount; col++)
        {
          var header = headerRow[col];
          if (header?.ToLower().Contains("tổng cộng") == true || header?.ToLower().Contains("tong cong") == true || header?.ToLower().Contains("total") == true)
          {
            hasTotalColumn = true;
            totalColumnIndex = col;
          }
          if (validColumns.Contains(header) && !validColumnIndices.Contains(col))
          {
            validColumnIndices.Add(col);
          }
        }
        if (!hasTotalColumn)
        {
          colCount++;
          totalColumnIndex = colCount - 1;
        }

        var planCells = new List<PlanCell>(rowCount * colCount);
        for (int row = 1; row <= rowCount; row++)
        {
          var rowData = new List<object>(colCount);
          var isNewRow = row == rowCount && row == lastNonEmptyRow + 1;

          for (int col = 1; col <= colCount; col++)
          {
            if (isNewRow)
            {
              rowData.Add("");
              var cell = new PlanCell
              {
                PlanId = planId,
                Name = "",
                RowId = row,
                ColumnId = col,
                BackgroundColor = col == totalColumnIndex + 1 ? "f0f0f0" : "ffffff",
                FontColor = "000000",
                FontSize = "11pt",
                FontWeight = "normal",
                TextAlign = col == totalColumnIndex + 1 ? "center" : "left",
                FontFamily = col == totalColumnIndex + 1 ? "Times New Roman" : "Arial",
                InputSettings = "",
                Rowspan = 1,
                Colspan = 1,
                IsHidden = false,
                IsFileUpload = false,
                RowHeight = 30,
                ColWidth = 60,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                IsLocked = false
              };
              planCells.Add(cell);
            }
            else
            {
              var cellValue = worksheet.Cells[row, col].Text?.Trim() ?? "";
              rowData.Add(cellValue);
              var cell = new PlanCell
              {
                PlanId = planId,
                Name = cellValue,
                RowId = row,
                ColumnId = col,
                BackgroundColor = GetCellBackgroundColor(worksheet.Cells[row, col]),
                FontColor = GetCellFontColor(worksheet.Cells[row, col]),
                FontSize = worksheet.Cells[row, col].Style.Font.Size.ToString() + "pt",
                FontWeight = worksheet.Cells[row, col].Style.Font.Bold ? "bold" : "normal",
                TextAlign = worksheet.Cells[row, col].Style.HorizontalAlignment.ToString().ToLower(),
                FontFamily = worksheet.Cells[row, col].Style.Font.Name ?? "Arial",
                InputSettings = "",
                Rowspan = 1,
                Colspan = 1,
                IsHidden = false,
                IsFileUpload = false,
                RowHeight = worksheet.Row(row).Height > 0 ? worksheet.Row(row).Height : 30,
                ColWidth = worksheet.Column(col).Width > 0 ? worksheet.Column(col).Width * 7.5 : 60,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                IsLocked = false
              };
              planCells.Add(cell);
            }
          }
          tableData.Add(rowData);
        }

        foreach (var mergedRange in worksheet.MergedCells)
        {
          var range = worksheet.Cells[mergedRange];
          var startRow = range.Start.Row;
          var startCol = range.Start.Column;
          var endRow = range.End.Row;
          var endCol = range.End.Column;
          if (startRow <= lastNonEmptyRow && startCol <= colCount && endRow <= lastNonEmptyRow && endCol <= colCount)
          {
            var cell = planCells.FirstOrDefault(c => c.RowId == startRow && c.ColumnId == startCol);
            if (cell != null)
            {
              cell.Rowspan = endRow - startRow + 1;
              cell.Colspan = endCol - startCol + 1;
              cell.TextAlign = "center";
            }
            mergedCells.Add(new Dictionary<string, object>
                {
                    { "startRow", startRow },
                    { "startCol", startCol },
                    { "rowSpan", endRow - startRow + 1 },
                    { "colSpan", endCol - startCol + 1 }
                });
          }
        }

        for (int row = 2; row <= lastNonEmptyRow; row++)
        {
          double total = 0;
          foreach (int colIndex in validColumnIndices)
          {
            var cellValue = worksheet.Cells[row, colIndex + 1].Text?.Trim();
            if (double.TryParse(cellValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value))
            {
              total += value;
            }
          }
          tableData[row - 1][totalColumnIndex] = Math.Floor(total) == total ? total.ToString("0") : total.ToString("0.##");
        }
        if (rowCount > lastNonEmptyRow)
        {
          tableData[rowCount - 1][totalColumnIndex] = "0";
        }

        for (int row = 1; row <= rowCount; row++)
        {
          var isNewRow = row == rowCount && row == lastNonEmptyRow + 1;
          rowHeights.Add(isNewRow ? 30 : worksheet.Row(row).Height > 0 ? worksheet.Row(row).Height : 30);
          var rowFormats = new Dictionary<string, string>();
          for (int col = 1; col <= colCount; col++)
          {
            var format = new Dictionary<string, string>
                {
                    { "backgroundColor", isNewRow ? (col == totalColumnIndex + 1 ? "f0f0f0" : "ffffff") : GetCellBackgroundColor(worksheet.Cells[row, col]) },
                    { "fontColor", isNewRow ? "000000" : GetCellFontColor(worksheet.Cells[row, col]) },
                    { "fontSize", isNewRow ? "11pt" : worksheet.Cells[row, col].Style.Font.Size.ToString() + "pt" },
                    { "fontWeight", isNewRow ? "normal" : (worksheet.Cells[row, col].Style.Font.Bold ? "bold" : "normal") },
                    { "textAlign", isNewRow ? (col == totalColumnIndex + 1 ? "center" : "left") : worksheet.Cells[row, col].Style.HorizontalAlignment.ToString().ToLower() },
                    { "fontFamily", isNewRow ? (col == totalColumnIndex + 1 ? "Times New Roman" : "Arial") : (worksheet.Cells[row, col].Style.Font.Name ?? "Arial") }
                };
            rowFormats[$"col{col}"] = ConvertFormatToCss(format);
          }
          cellFormats.Add(rowFormats);
        }

        for (int col = 1; col <= colCount; col++)
        {
          var column = worksheet.Column(col);
          double excelWidth = column.Width > 0 ? column.Width : 8.43;
          double pixelWidth = Math.Round(excelWidth * 7.5);
          colWidths.Add(Math.Max(pixelWidth, 60));
        }

        await _detailKehoachService.AddPlanCellsAsync(planCells);

        return Json(new
        {
          success = true,
          data = tableData,
          formats = cellFormats,
          mergedCells = mergedCells,
          rowHeights = rowHeights,
          colWidths = colWidths,
          totalColumnIndex = totalColumnIndex,
          validColumnIndices = validColumnIndices
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
      }
    }

    private int GetLastNonEmptyRow(ExcelWorksheet worksheet, int colCount)
    {
      int maxRow = worksheet.Dimension?.End.Row ?? 0;
      for (int row = maxRow; row >= 1; row--)
      {
        for (int col = 1; col <= colCount; col++)
        {
          var cellValue = worksheet.Cells[row, col].Value;
          if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
          {
            return row;
          }
        }
      }
      return 0;
    }
    [HttpPost]
    public async Task<IActionResult> AddColumn([FromBody] List<PlanCell> planCells)
    {
      try
      {
        if (planCells == null || !planCells.Any())
        {
          return Json(new { success = false, message = "Danh sách ô không hợp lệ." });
        }
        await _detailKehoachService.AddPlanCellsAsync(planCells);
        return Json(new { success = true, message = "Thêm cột thành công." });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Lỗi khi thêm cột: " + ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> AddRow([FromBody] List<PlanCell> planCells)
    {
      try
      {
        if (planCells == null || !planCells.Any())
        {
          return Json(new { success = false, message = "Danh sách ô không hợp lệ." });
        }
        foreach (var cell in planCells)
        {
          if (cell.RowId <= 0 || cell.ColumnId <= 0)
          {
            return Json(new { success = false, message = "RowId hoặc ColumnId không hợp lệ." });
          }
          cell.Name = "";
        }
        await _detailKehoachService.AddPlanCellsAsync(planCells);
        return Json(new { success = true, message = "Thêm dòng thành công." });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Lỗi khi thêm dòng: " + ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCell([FromBody] List<PlanCell> planCells)
    {
      try
      {
        if (planCells == null || !planCells.Any() || planCells.Any(cell => cell == null || cell.PlanId <= 0 || cell.RowId <= 0 || cell.ColumnId <= 0))
        {
          return Json(new { success = false, message = "Dữ liệu ô không hợp lệ." });
        }
        foreach (var planCell in planCells)
        {
          await _detailKehoachService.UpdatePlanCellAsync(planCell);
        }
        return Json(new
        {
          success = true,
          message = "Cập nhật các ô thành công."
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi cập nhật ô: {ex.Message}. Inner Exception: {ex.InnerException?.Message}" });
      }
    }

    [HttpPost]
    public async Task<IActionResult> LockRows([FromBody] LockRowsRequest request)
    {
      try
      {
        if (request.PlanId <= 0 || request.SelectedRows == null || !request.SelectedRows.Any())
        {
          return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

        var result = await _detailKehoachService.LockRowsAsync(request.PlanId, request.SelectedRows);
        return Json(new
        {
          success = true,
          tableData = result.TableData,
          formats = result.Formats,
          lockedCells = result.LockedCells,
          message = "Khóa dòng thành công."
        });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi khóa dòng: {ex.Message}" });
      }
    }

    private string GetCellBackgroundColor(ExcelRange cell)
    {
      var fill = cell.Style.Fill;
      if (fill.PatternType == ExcelFillStyle.Solid && fill.BackgroundColor != null)
      {
        if (!string.IsNullOrEmpty(fill.BackgroundColor.Rgb))
        {
          string rgb = fill.BackgroundColor.Rgb;
          return rgb.Length == 8 ? rgb.Substring(2).ToLower() : rgb.ToLower();
        }
        else if (fill.BackgroundColor.Theme != null)
        {
          var themeColor = fill.BackgroundColor.LookupColor();
          return themeColor?.Length == 8 ? themeColor.Substring(2).ToLower() : themeColor?.ToLower() ?? "ffffff";
        }
      }
      return "ffffff";
    }

    private string GetCellFontColor(ExcelRange cell)
    {
      var fontColor = cell.Style.Font.Color;
      if (!string.IsNullOrEmpty(fontColor.Rgb))
      {
        string rgb = fontColor.Rgb;
        return rgb.Length == 8 ? rgb.Substring(2).ToLower() : rgb.ToLower();
      }
      else if (fontColor.Theme != null)
      {
        var themeColor = fontColor.LookupColor();
        return themeColor?.Length == 8 ? themeColor.Substring(2).ToLower() : themeColor?.ToLower() ?? "000000";
      }
      return "000000";
    }

    private string ConvertFormatToCss(Dictionary<string, string> format)
    {
      var css = new List<string>();
      if (!string.IsNullOrEmpty(format["backgroundColor"]))
        css.Add($"background-color: #{format["backgroundColor"]}");
      if (!string.IsNullOrEmpty(format["fontColor"]))
        css.Add($"color: #{format["fontColor"]}");
      css.Add($"font-size: {format["fontSize"]}");
      css.Add($"font-weight: {format["fontWeight"]}");
      css.Add($"text-align: {format["textAlign"]}");
      css.Add($"font-family: {format["fontFamily"]}");
      return string.Join("; ", css);
    }
  }

  public class LockRowsRequest
  {
    public int PlanId { get; set; }
    public List<int> SelectedRows { get; set; }
  }
}
