using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
using AspnetCoreMvcFull.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLANMHE.Repository
{
  public class THPlanRepository : ITHPlanRepository
  {
    private readonly ApplicationDbContext _context;

    public THPlanRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public List<Plan> GetPlansByUserId(int userId)
    {
      return _context.UserPlans
          .Where(up => up.UserId == userId)
          .Include(up => up.Plan)
          .Select(up => up.Plan)
          .ToList();
    }

    public List<Plan> GetAllPlans()
    {
      return _context.Plans.ToList();
    }

    public async Task<Plan> GetPlanById(int planId)
    {
      return await _context.Plans
          .FirstOrDefaultAsync(p => p.Id == planId);
    }

    public async Task<List<User>> GetAssignedUsersByPlanId(int planId)
    {
      return await _context.UserPlans
          .Where(up => up.PlanId == planId)
          .Include(up => up.User)
          .Select(up => up.User)
          .ToListAsync();
    }

    public async Task<List<PlanCell>> GetPlanCellsByPlanId(int planId)
    {
      return await _context.PlanCells
          .Where(pc => pc.PlanId == planId && !pc.IsDeleted)
          .ToListAsync();
    }

    public async Task UpdatePlanCellAsync(PlanCell planCell)
    {
      var existingCell = await _context.PlanCells
          .FirstOrDefaultAsync(pc => pc.PlanId == planCell.PlanId &&
                                   pc.RowId == planCell.RowId &&
                                   pc.ColumnId == planCell.ColumnId &&
                                   !pc.IsDeleted);
      if (existingCell != null)
      {
        existingCell.Name = planCell.Name;
        existingCell.BackgroundColor = planCell.BackgroundColor;
        existingCell.FontColor = planCell.FontColor;
        existingCell.FontSize = planCell.FontSize;
        existingCell.FontWeight = planCell.FontWeight;
        existingCell.TextAlign = planCell.TextAlign;
        existingCell.FontFamily = planCell.FontFamily;
        existingCell.Rowspan = planCell.Rowspan;
        existingCell.Colspan = planCell.Colspan;
        existingCell.RowHeight = planCell.RowHeight;
        existingCell.ColWidth = planCell.ColWidth;
        existingCell.IsHidden = planCell.IsHidden;
        existingCell.IsFileUpload = planCell.IsFileUpload;
        existingCell.IsLocked = planCell.IsLocked;
      }
      else
      {
        _context.PlanCells.Add(planCell);
      }
      await _context.SaveChangesAsync();
    }
    public async Task AddRowAsync(int planId, int rowId)
    {
      var planCells = await _context.PlanCells
          .Where(pc => pc.PlanId == planId && !pc.IsDeleted)
          .ToListAsync();
      int maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 1;
      for (int col = 1; col <= maxCol; col++)
      {
        var newCell = new PlanCell
        {
          PlanId = planId,
          RowId = rowId,
          ColumnId = col,
          Name = "",
          BackgroundColor = "ffffff",
          FontColor = "000000",
          FontSize = "14px",
          FontWeight = "normal",
          TextAlign = "left",
          FontFamily = "Segoe UI",
          Rowspan = 1,
          Colspan = 1,
          RowHeight = 30,
          ColWidth = planCells.FirstOrDefault(pc => pc.ColumnId == col)?.ColWidth ?? 60,
          InputSettings = "",
          IsHidden = false,
          IsFileUpload = false,
          IsDeleted = false,
          IsLocked = false
        };
        _context.PlanCells.Add(newCell);
      }
      await _context.SaveChangesAsync();
    }

    public async Task AddColumnAsync(int planId, int columnId)
    {
      var planCells = await _context.PlanCells
          .Where(pc => pc.PlanId == planId && !pc.IsDeleted)
          .ToListAsync();
      int maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 1;
      for (int row = 1; row <= maxRow; row++)
      {
        var newCell = new PlanCell
        {
          PlanId = planId,
          RowId = row,
          ColumnId = columnId,
          Name = row == 1 ? "New Column" : "",
          BackgroundColor = "ffffff",
          FontColor = "000000",
          FontSize = "14px",
          FontWeight = "normal",
          TextAlign = "left",
          FontFamily = "Segoe UI",
          Rowspan = 1,
          Colspan = 1,
          RowHeight = planCells.FirstOrDefault(pc => pc.RowId == row)?.RowHeight ?? 30,
          ColWidth = 60,
          InputSettings = "",
          IsHidden = false,
          IsFileUpload = false,
          IsDeleted = false,
          IsLocked = false
        };
        _context.PlanCells.Add(newCell);
      }
      await _context.SaveChangesAsync();
    }

    public async Task DeleteRowAsync(int planId, int rowId)
    {
      var cells = await _context.PlanCells
          .Where(pc => pc.PlanId == planId && pc.RowId == rowId && !pc.IsDeleted)
          .ToListAsync();
      foreach (var cell in cells)
      {
        cell.IsDeleted = true;
      }
      await _context.SaveChangesAsync();
    }

    public async Task DeleteColumnAsync(int planId, int columnId)
    {
      var cells = await _context.PlanCells
          .Where(pc => pc.PlanId == planId && pc.ColumnId == columnId && !pc.IsDeleted)
          .ToListAsync();
      foreach (var cell in cells)
      {
        cell.IsDeleted = true;
      }
      await _context.SaveChangesAsync();
    }

    public async Task ConfirmPlanAsync(int planId)
    {
      var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == planId);
      if (plan == null)
        throw new Exception("Không tìm thấy kế hoạch!");
      if (plan.Status == "Completed")
        throw new Exception("Kế hoạch đã hoàn thành rồi!");
      plan.Status = "Completed";
      await _context.PlanCells
          .Where(pc => pc.PlanId == planId && !pc.IsDeleted && !pc.IsHidden)
          .ExecuteUpdateAsync(setters => setters.SetProperty(pc => pc.IsLocked, true));
      await _context.SaveChangesAsync();
    }
  }
}
