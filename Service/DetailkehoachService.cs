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

    public async Task<LockRowsResult> LockRowsAsync(int planId, IEnumerable<int> selectedRows)
    {
      var planCells = (await _repository.GetPlanCellsAsync(planId)).ToList();
      var maxRow = planCells.Any() ? planCells.Max(pc => pc.RowId) : 0;
      var maxCol = planCells.Any() ? planCells.Max(pc => pc.ColumnId) : 0;
      if (maxRow == 0 || maxCol == 0)
      {
        throw new Exception("Không có dữ liệu bảng.");
      }

      // Tạo tableData, formats, lockedCells
      var tableData = new List<List<object>>();
      var formats = new List<Dictionary<string, string>>();
      var lockedCells = new List<Dictionary<string, bool>>();

      // Tạo từ điển để tra cứu nhanh PlanCell theo (RowId, ColumnId)
      var cellDictionary = planCells
          .GroupBy(pc => (pc.RowId, pc.ColumnId))
          .Select(g => g.First())
          .ToDictionary(pc => (pc.RowId, pc.ColumnId), pc => pc);

      // Khởi tạo tableData, formats, và lockedCells
      for (int row = 1; row <= maxRow; row++)
      {
        var rowData = new List<object>();
        var rowFormats = new Dictionary<string, string>();
        var rowLocked = new Dictionary<string, bool>();
        for (int col = 1; col <= maxCol; col++)
        {
          var cellKey = (row, col);
          var cell = cellDictionary.ContainsKey(cellKey) ? cellDictionary[cellKey] : null;
          rowData.Add(cell?.Name?.Trim() ?? "");
          rowLocked[$"col{col}"] = true; // Mặc định khóa tất cả
          rowFormats[$"col{col}"] = cell != null
              ? $"background-color: #{cell.BackgroundColor}; color: #{cell.FontColor}; font-size: {cell.FontSize}; font-weight: {cell.FontWeight}; text-align: {cell.TextAlign}; font-family: {cell.FontFamily}"
              : "background-color: #ffffff; color: #000000; font-size: 11pt; font-weight: normal; text-align: left; font-family: Arial";
        }
        tableData.Add(rowData);
        formats.Add(rowFormats);
        lockedCells.Add(rowLocked);
      }

      // Xác định các dòng kế tiếp cần mở khóa
      var nextRowsToEnable = selectedRows.Where(row => row < maxRow - 1).Select(row => row + 1).ToList();

      // Cập nhật trạng thái khóa/mở khóa
      var updatedCells = new List<PlanCell>();
      for (int row = 1; row <= maxRow; row++)
      {
        var isNextRow = nextRowsToEnable.Contains(row - 1);
        var isSelectedRow = selectedRows.Contains(row - 1);
        for (int col = 1; col <= maxCol; col++)
        {
          var cellKey = (row, col);
          var cell = cellDictionary.ContainsKey(cellKey) ? cellDictionary[cellKey] : new PlanCell
          {
            PlanId = planId,
            RowId = row,
            ColumnId = col,
            Name = "",
            BackgroundColor = "ffffff",
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
          // Logic khóa/mở khóa
          cell.IsLocked = !isNextRow; // Khóa tất cả trừ dòng kế tiếp của dòng được chọn
          if (isNextRow)
          {
            lockedCells[row - 1][$"col{col}"] = false; // Mở khóa dòng kế tiếp
          }
          updatedCells.Add(cell);
        }
      }

      // Cập nhật tất cả PlanCells vào database
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
