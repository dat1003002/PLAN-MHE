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
    // Thuộc tính điều hướng cho mối quan hệ nhiều-nhiều với User
    public ICollection<UserPlan> UserPlans { get; set; }
  }
}
