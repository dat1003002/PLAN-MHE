using System.ComponentModel.DataAnnotations;
namespace PLANMHE.Models
{
  public class PlanCell
  {
    [Key]
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public string InputSettings { get; set; }
    public int? Rowspan { get; set; }
    public int? Colspan { get; set; }
    public bool IsHidden { get; set; }
    public bool IsFileUpload { get; set; }
    [StringLength(50)]
    public string BackgroundColor { get; set; }
    public double RowHeight { get; set; }
    public double ColWidth { get; set; }
    public string FontColor { get; set; }
    public string FontSize { get; set; }
    public string FontWeight { get; set; }
    public string TextAlign { get; set; }
    public string FontFamily { get; set; }
    public bool IsLocked { get; set; } = false;
    public int PlanId { get; set; }
    public Plan Plan { get; set; }
  }
}
