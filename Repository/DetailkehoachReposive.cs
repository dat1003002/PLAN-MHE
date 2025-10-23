using AspnetCoreMvcFull.Data;
using Microsoft.EntityFrameworkCore;
using PLANMHE.Models;
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
                .FirstOrDefaultAsync(pc => pc.PlanId == planCell.PlanId && pc.RowId == planCell.RowId && pc.ColumnId == planCell.ColumnId && !pc.IsDeleted);

            if (existingCell != null)
            {
                existingCell.Name = planCell.Name;
                existingCell.BackgroundColor = planCell.BackgroundColor?.Substring(0, Math.Min(planCell.BackgroundColor?.Length ?? 50, 50)) ?? "ffffff";
                existingCell.FontColor = planCell.FontColor?.Substring(0, Math.Min(planCell.FontColor?.Length ?? 50, 50)) ?? "000000";
                existingCell.FontSize = planCell.FontSize ?? "11pt";
                existingCell.FontWeight = planCell.FontWeight ?? "normal";
                existingCell.TextAlign = planCell.TextAlign ?? "left";
                existingCell.FontFamily = planCell.FontFamily?.Substring(0, Math.Min(planCell.FontFamily?.Length ?? 50, 50)) ?? "Arial";
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
            if (!planCellList.Any())
            {
                return;
            }

            var planId = planCellList.First().PlanId;
            var existingCells = await _context.PlanCells
                .Where(pc => pc.PlanId == planId && !pc.IsDeleted)
                .ToDictionaryAsync(pc => (pc.RowId, pc.ColumnId), pc => pc);

            foreach (var planCell in planCellList)
            {
                var key = (planCell.RowId, planCell.ColumnId);
                if (existingCells.TryGetValue(key, out var existingCell))
                {
                    existingCell.Name = planCell.Name;
                    existingCell.BackgroundColor = planCell.BackgroundColor?.Substring(0, Math.Min(planCell.BackgroundColor?.Length ?? 50, 50)) ?? "ffffff";
                    existingCell.FontColor = planCell.FontColor?.Substring(0, Math.Min(planCell.FontColor?.Length ?? 50, 50)) ?? "000000";
                    existingCell.FontSize = planCell.FontSize ?? "11pt";
                    existingCell.FontWeight = planCell.FontWeight ?? "normal";
                    existingCell.TextAlign = planCell.TextAlign ?? "left";
                    existingCell.FontFamily = planCell.FontFamily?.Substring(0, Math.Min(planCell.FontFamily?.Length ?? 50, 50)) ?? "Arial";
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
            }

            await _context.SaveChangesAsync();
        }
    }
}
