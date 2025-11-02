using PLANMHE.Models;
using PLANMHE.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLANMHE.Service
{
  public class DetailkehoachService : IDetailkehoachService
  {
    private readonly IDetailkehoachReposive _repository;

    public DetailkehoachService(IDetailkehoachReposive repository)
    {
      _repository = repository;
    }

    public async Task<int> AddPlanAsync(Plan plan, IEnumerable<int> userIds)
    {
      return await _repository.AddPlanAsync(plan, userIds);
    }

    public async Task AddPlanCellsAsync(IEnumerable<PlanCell> planCells)
    {
      await _repository.AddPlanCellsAsync(planCells);
    }

    public async Task<IEnumerable<PlanCell>> GetPlanCellsAsync(int planId)
    {
      return await _repository.GetPlanCellsAsync(planId);
    }

    public async Task UpdatePlanCellAsync(PlanCell planCell)
    {
      await _repository.UpdatePlanCellAsync(planCell);
    }

    public async Task UpdatePlanCellsAsync(IEnumerable<PlanCell> planCells)
    {
      await _repository.UpdatePlanCellsAsync(planCells);
    }

    public async Task<LockRowsResult> LockRowsAsync(int planId, IEnumerable<int> selectedRows)
    {
      var planCells = (await _repository.GetPlanCellsAsync(planId)).ToList();
      var maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
      var maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
      if (maxRow == 0 || maxCol == 0)
      {
        throw new Exception("Không có dữ liệu bảng.");
      }
      var cellDictionary = planCells
          .GroupBy(pc => (pc.RowId, pc.ColumnId))
          .Select(g => g.First())
          .ToDictionary(pc => (pc.RowId, pc.ColumnId), pc => pc);
      var nextRowsToEnable = selectedRows.Where(row => row < maxRow - 1).Select(row => row + 1).ToList();
      var tableData = new List<List<object>>();
      var formats = new List<Dictionary<string, string>>();
      var lockedCells = new List<Dictionary<string, bool>>();
      var updatedCells = new List<PlanCell>();
      for (int row = 1; row <= maxRow; row++)
      {
        var rowData = new List<object>();
        var rowFormats = new Dictionary<string, string>();
        var rowLocked = new Dictionary<string, bool>();
        var isNextRow = nextRowsToEnable.Contains(row - 1);
        for (int col = 1; col <= maxCol; col++)
        {
          var cellKey = (row, col);
          var cell = cellDictionary.ContainsKey(cellKey) ? cellDictionary[cellKey] : new PlanCell
          {
            PlanId = planId,
            RowId = row,
            ColumnId = col,
            Name = "",
            BackgroundColor = "ffffff",  // FALLBACK TRẮNG TỪ DB
            FontColor = "000000",
            FontSize = "11pt",
            FontWeight = "normal",
            TextAlign = "left",
            FontFamily = "Arial",
            Rowspan = 1,
            Colspan = 1,
            RowHeight = 30,
            ColWidth = 60,
            InputSettings = "",
            IsHidden = false,
            IsFileUpload = false,
            IsDeleted = false
          };
          rowData.Add(cell.Name?.Trim() ?? "");
          bool isLocked = !isNextRow;
          rowLocked[$"col{col}"] = isLocked;
          cell.IsLocked = isLocked;
          if (isNextRow)
          {
            rowLocked[$"col{col}"] = false;
          }
          var baseCss = $"background-color: #{cell.BackgroundColor ?? "ffffff"}; color: #{cell.FontColor ?? "000000"}; font-size: {cell.FontSize ?? "11pt"}; font-weight: {cell.FontWeight ?? "normal"}; text-align: {cell.TextAlign ?? "left"}; font-family: {cell.FontFamily ?? "Arial"}";
          var fullCss = isLocked ? $"{baseCss}; cursor: not-allowed" : baseCss;  // CHỈ THÊM CURSOR, GIỮ MÀU DB
          rowFormats[$"col{col}"] = fullCss;
          updatedCells.Add(cell);
        }
        tableData.Add(rowData);
        formats.Add(rowFormats);
        lockedCells.Add(rowLocked);
      }
      await _repository.UpdatePlanCellsAsync(updatedCells);
      return new LockRowsResult
      {
        TableData = tableData,
        Formats = formats,
        LockedCells = lockedCells
      };
    }
  }
}
