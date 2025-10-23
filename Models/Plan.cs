using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PLANMHE.Models
{
  public class Plan
  {
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    [StringLength(500)]
    public string? Description { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime EndDate { get; set; }
    [Required]
    public string Status { get; set; } = "Active";
    public ICollection<UserPlan> UserPlans { get; set; }
    public ICollection<PlanCell> PlanCells { get; set; }
  }
}
