using AspnetCoreMvcFull.Data;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLANMHE.Repository
{
  public class DetailkehoachReposive : IDetailkehoachReposive
  {
    private readonly ApplicationDbContext _context;

    public DetailkehoachReposive(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<int> AddPlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      _context.Plans.Add(plan);
      await _context.SaveChangesAsync();

      foreach (var userId in userIds)
      {
        _context.UserPlans.Add(new UserPlan { PlanId = plan.Id, UserId = userId });
      }

      await _context.SaveChangesAsync();
      return plan.Id;
    }

    public async Task AddPlanCellsAsync(IEnumerable<PlanCell> planCells)
    {
      foreach (var cell in planCells)
      {
        cell.InputSettings = cell.InputSettings ?? "";
      }
      _context.PlanCells.AddRange(planCells);
      await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PlanCell>> GetPlanCellsAsync(int planId)
    {
      return await _context.PlanCells
          .Where(pc => pc.PlanId == planId && !pc.IsDeleted)
          .ToListAsync();
    }

    public async Task UpdatePlanCellAsync(PlanCell planCell)
    {
      var planExists = await _context.Plans.AnyAsync(p => p.Id == planCell.PlanId);
      if (!planExists)
      {
        throw new Exception($"Plan with ID {planCell.PlanId} does not exist.");
      }

      var existingCell = await _context.PlanCells
          .FirstOrDefaultAsync(pc => pc.PlanId == planCell.PlanId
                                  && pc.RowId == planCell.RowId
                                  && pc.ColumnId == planCell.ColumnId
                                  && !pc.IsDeleted);

      if (existingCell != null)
      {
        // Cập nhật từng field – an toàn, không dùng ExecuteUpdate
        existingCell.Name = planCell.Name;
        existingCell.BackgroundColor = TruncateColor(planCell.BackgroundColor, "ffffff");
        existingCell.FontColor = TruncateColor(planCell.FontColor, "000000");
        existingCell.FontSize = planCell.FontSize ?? "11pt";
        existingCell.FontWeight = planCell.FontWeight ?? "normal";
        existingCell.TextAlign = planCell.TextAlign ?? "left";
        existingCell.FontFamily = planCell.FontFamily?.Length > 50
            ? planCell.FontFamily.Substring(0, 50)
            : (planCell.FontFamily ?? "Arial");
        existingCell.Rowspan = planCell.Rowspan ?? 1;
        existingCell.Colspan = planCell.Colspan ?? 1;
        existingCell.RowHeight = planCell.RowHeight > 0 ? planCell.RowHeight : 30;
        existingCell.ColWidth = planCell.ColWidth > 0 ? planCell.ColWidth : 60;
        existingCell.InputSettings = planCell.InputSettings ?? "";
        existingCell.IsHidden = planCell.IsHidden;
        existingCell.IsFileUpload = planCell.IsFileUpload;
        existingCell.IsDeleted = planCell.IsDeleted;
        existingCell.IsLocked = planCell.IsLocked;
      }
      else
      {
        _context.PlanCells.Add(planCell);
      }

      await _context.SaveChangesAsync();
    }

    public async Task UpdatePlanCellsAsync(IEnumerable<PlanCell> planCells)
    {
      var planCellList = planCells.ToList();
      if (!planCellList.Any()) return;

      var planId = planCellList.First().PlanId;

      // Lấy tất cả cell theo PlanId → Dictionary theo (RowId, ColumnId)
      var existingCells = await _context.PlanCells
          .Where(pc => pc.PlanId == planId && !pc.IsDeleted)
          .ToDictionaryAsync(pc => (pc.RowId, pc.ColumnId));

      foreach (var cell in planCellList)
      {
        var key = (cell.RowId, cell.ColumnId);
        if (existingCells.TryGetValue(key, out var existing))
        {
          // Cập nhật các field cần thiết
          existing.Name = cell.Name;
          existing.BackgroundColor = TruncateColor(cell.BackgroundColor, "ffffff");
          existing.FontColor = TruncateColor(cell.FontColor, "000000");
          existing.FontSize = cell.FontSize ?? "11pt";
          existing.FontWeight = cell.FontWeight ?? "normal";
          existing.TextAlign = cell.TextAlign ?? "left";
          existing.FontFamily = cell.FontFamily ?? "Arial";
          existing.Rowspan = cell.Rowspan ?? 1;
          existing.Colspan = cell.Colspan ?? 1;
          existing.RowHeight = cell.RowHeight > 0 ? cell.RowHeight : 30;
          existing.ColWidth = cell.ColWidth > 0 ? cell.ColWidth : 60;
          existing.InputSettings = cell.InputSettings ?? "";
          existing.IsHidden = cell.IsHidden;
          existing.IsFileUpload = cell.IsFileUpload;
          existing.IsDeleted = cell.IsDeleted;
          existing.IsLocked = cell.IsLocked;
        }
        else
        {
          // Thêm mới nếu chưa tồn tại
          _context.PlanCells.Add(cell);
        }
      }

      await _context.SaveChangesAsync();
    }

    // Hàm hỗ trợ: Cắt chuỗi màu về 6 ký tự, fallback nếu null
    private string TruncateColor(string color, string fallback)
    {
      if (string.IsNullOrEmpty(color)) return fallback;
      return color.Length > 6 ? color.Substring(0, 6) : color.PadRight(6, '0');
    }
  }
}
