using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
using PLANMHE.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AspnetCoreMvcFull.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace PLANMHE.Controllers
{
  [Authorize]
  public class HistoryPlanController : Controller
  {
    private readonly ILichSuPlanService _service;
    private readonly ITHPlanService _thPlanService;
    private readonly ApplicationDbContext _context;

    public HistoryPlanController(ILichSuPlanService service, ITHPlanService thPlanService, ApplicationDbContext context)
    {
      _service = service;
      _thPlanService = thPlanService;
      _context = context;
    }

    public IActionResult ListLichsuKeHoach(int pageNumber = 1)
    {
      const int pageSize = 10;
      var plans = _service.GetPlansForList(pageNumber, pageSize);
      var totalItems = _service.GetTotalPlanCount();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
      pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));
      ViewBag.TotalPages = totalPages;
      ViewBag.CurrentPage = pageNumber;
      return View("~/Views/HistoryPlan/ListLichsuKeHoach.cshtml", plans);
    }

    public async Task<IActionResult> Detail(int id)
    {
      try
      {
        var plan = _service.GetPlanById(id);
        if (plan == null) return NotFound();

        var assignedUsers = _context.UserPlans
            .Where(up => up.PlanId == id)
            .Include(up => up.User)
            .Select(up => up.User)
            .ToList();
        ViewBag.AssignedUsers = assignedUsers;

        var planCells = await _thPlanService.GetPlanCellsByPlanId(id);
        bool hasLockedCells = planCells.Any(pc => pc.IsLocked && !pc.IsDeleted && !pc.IsHidden);
        if (!hasLockedCells && plan.Status != "Completed")
          return RedirectToAction("ListLichsuKeHoach");

        int totalColumnIndex = -1;
        var validColumnIndices = new List<int>();
        string[] validColumns = { "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN", "Thứ 2", "Thứ 3" };
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden).ToList();
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
            totalColumnIndex = cell.ColumnId - 1;
          if (validColumns.Contains(cell.Name.Trim()) && !validColumnIndices.Contains(cell.ColumnId - 1))
            validColumnIndices.Add(cell.ColumnId - 1);
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
          // Dữ liệu bảng
          for (int row = 1; row <= maxRow; row++)
          {
            var rowData = new List<object>();
            for (int col = 1; col <= maxCol; col++)
            {
              var cell = planCells.FirstOrDefault(pc => pc.RowId == row && pc.ColumnId == col && !pc.IsDeleted && !pc.IsHidden);
              rowData.Add(cell?.Name?.Trim() ?? "");
            }
            tableData.Add(rowData);
          }

          // Định dạng
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
              if (col - 1 == totalColumnIndex || cell?.IsLocked == true)
                css.Add("cursor: not-allowed");
              rowFormats[$"col{col}"] = string.Join("; ", css);
            }
            cellFormats.Add(rowFormats);
          }

          // Gộp ô
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

          // Ô khóa
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

        return View("~/Views/HistoryPlan/DetailLichSuPlan.cshtml", plan);
      }
      catch (Exception ex)
      {
        return StatusCode(500, "Lỗi khi lấy chi tiết kế hoạch: " + ex.Message);
      }
    }

    [HttpGet]
    public async Task<IActionResult> ExportToExcel(int planId)
    {
      try
      {
        var plan = _service.GetPlanById(planId);
        if (plan == null) return NotFound();

        var planCells = await _thPlanService.GetPlanCellsByPlanId(planId);
        if (!planCells.Any()) return BadRequest("Không có dữ liệu để xuất!");

        int maxRow = planCells.Max(pc => pc.RowId);
        int maxCol = planCells.Max(pc => pc.ColumnId);

        // Tìm cột tổng cộng
        int totalColumnIndex = -1;
        var headerCells = planCells.Where(pc => pc.RowId == 1 && !pc.IsDeleted && !pc.IsHidden);
        foreach (var cell in headerCells)
        {
          if (cell.Name.ToLower().Contains("tổng cộng") || cell.Name.ToLower().Contains("total"))
          {
            totalColumnIndex = cell.ColumnId - 1;
            break;
          }
        }

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Plan");

        // 1. Đổ dữ liệu
        for (int r = 1; r <= maxRow; r++)
          for (int c = 1; c <= maxCol; c++)
          {
            var cell = planCells.FirstOrDefault(pc => pc.RowId == r && pc.ColumnId == c && !pc.IsDeleted && !pc.IsHidden);
            worksheet.Cells[r, c].Value = cell?.Name?.Trim() ?? "";
          }

        // 2. ĐỊNH DẠNG CHI TIẾT
        foreach (var cell in planCells.Where(pc => !pc.IsDeleted && !pc.IsHidden))
        {
          var excelCell = worksheet.Cells[cell.RowId, cell.ColumnId];

          // MÀU NỀN
          string bgHex = cell.BackgroundColor?.Trim();
          if (!string.IsNullOrEmpty(bgHex))
          {
            if (!bgHex.StartsWith("#")) bgHex = "#" + bgHex;
            try
            {
              var bgColor = ColorTranslator.FromHtml(bgHex);
              excelCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
              excelCell.Style.Fill.BackgroundColor.SetColor(bgColor);
            }
            catch { }
          }
          else if (totalColumnIndex >= 0 && cell.ColumnId - 1 == totalColumnIndex)
          {
            excelCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            excelCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 240, 240));
          }

          // MÀU CHỮ
          string fgHex = cell.FontColor?.Trim();
          if (!string.IsNullOrEmpty(fgHex))
          {
            if (!fgHex.StartsWith("#")) fgHex = "#" + fgHex;
            try { excelCell.Style.Font.Color.SetColor(ColorTranslator.FromHtml(fgHex)); }
            catch { }
          }

          // FONT SIZE
          if (float.TryParse(cell.FontSize?.Replace("px", ""), out float pxSize))
            excelCell.Style.Font.Size = pxSize * 0.75f;
          else
            excelCell.Style.Font.Size = 10.5f;

          // BOLD
          excelCell.Style.Font.Bold = cell.FontWeight == "bold" ||
              (int.TryParse(cell.FontWeight, out int w) && w > 600);

          // FONT FAMILY
          excelCell.Style.Font.Name = string.IsNullOrEmpty(cell.FontFamily) ? "Segoe UI" : cell.FontFamily;

          // CĂN LỀ
          excelCell.Style.HorizontalAlignment = cell.TextAlign switch
          {
            "center" => ExcelHorizontalAlignment.Center,
            "right" => ExcelHorizontalAlignment.Right,
            _ => ExcelHorizontalAlignment.Left
          };
          excelCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        // 2.5. THÊM VIỀN CHO TOÀN BỘ LƯỚI (GRID)
        var dataRange = worksheet.Cells[1, 1, maxRow, maxCol];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Top.Color.SetColor(Color.Black);
        dataRange.Style.Border.Bottom.Color.SetColor(Color.Black);
        dataRange.Style.Border.Left.Color.SetColor(Color.Black);
        dataRange.Style.Border.Right.Color.SetColor(Color.Black);

        // Viền ngoài cùng đậm hơn (tô đậm khung)
        worksheet.Cells[1, 1, 1, maxCol].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        worksheet.Cells[maxRow, 1, maxRow, maxCol].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        worksheet.Cells[1, 1, maxRow, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        worksheet.Cells[1, maxCol, maxRow, maxCol].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        // 3. GỘP Ô
        foreach (var cell in planCells.Where(pc => (pc.Rowspan > 1 || pc.Colspan > 1) && !pc.IsDeleted && !pc.IsHidden))
        {
          int endRow = cell.RowId + (cell.Rowspan ?? 1) - 1;
          int endCol = cell.ColumnId + (cell.Colspan ?? 1) - 1;
          worksheet.Cells[cell.RowId, cell.ColumnId, endRow, endCol].Merge = true;
        }

        // 4. CHIỀU RỘNG & CAO
        var colWidths = planCells.GroupBy(pc => pc.ColumnId)
            .Select(g => g.First().ColWidth > 0 ? g.First().ColWidth : 60).ToList();
        var rowHeights = planCells.GroupBy(pc => pc.RowId)
            .Select(g => g.First().RowHeight > 0 ? g.First().RowHeight : 30).ToList();

        for (int i = 0; i < colWidths.Count; i++)
          worksheet.Column(i + 1).Width = colWidths[i] / 7.0;
        for (int i = 0; i < rowHeights.Count; i++)
          worksheet.Row(i + 1).Height = rowHeights[i] * 0.75;

        // 5. XUẤT FILE
        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        var fileName = $"KeHoach_{planId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
      }
      catch (Exception ex)
      {
        Console.WriteLine("=== EXPORT EXCEL ERROR ===");
        Console.WriteLine(ex.ToString());
        return StatusCode(500, "Lỗi xuất Excel: " + ex.Message);
      }
    }
  }
}
