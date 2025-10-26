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
using AspnetCoreMvcFull.Data;

namespace PLANMHE.Controllers
{
  [Authorize]
  public class HistoryPlanController : Controller
  {
    private readonly ILichSuPlanService _service;
    private readonly ITHPlanService _thPlanService; // Thêm service để xử lý PlanCell
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
      var allPlans = _service.GetAllPlans();
      var totalItems = allPlans.Count();
      var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
      pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
      var plans = allPlans
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToList();
      ViewBag.TotalPages = totalPages;
      ViewBag.CurrentPage = pageNumber;
      return View("~/Views/HistoryPlan/ListLichsuKeHoach.cshtml", plans);
    }

    public async Task<IActionResult> Detail(int id)
    {
      try
      {
        var plan = _service.GetPlanById(id);
        if (plan == null)
        {
          return NotFound();
        }

        var assignedUsers = _context.UserPlans
            .Where(up => up.PlanId == id)
            .Include(up => up.User)
            .Select(up => up.User)
            .ToList();
        ViewBag.AssignedUsers = assignedUsers;

        var planCells = await _thPlanService.GetPlanCellsByPlanId(id);
        bool hasLockedCells = planCells.Any(pc => pc.IsLocked && !pc.IsDeleted && !pc.IsHidden);
        if (!hasLockedCells && plan.Status != "Completed")
        {
          return RedirectToAction("ListLichsuKeHoach");
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

        return View("~/Views/HistoryPlan/DetailLichSuPlan.cshtml", plan);
      }
      catch (Exception ex)
      {
        return StatusCode(500, "Lỗi khi lấy chi tiết kế hoạch: " + ex.Message);
      }
    }
  }
}
