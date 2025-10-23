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
      [Required]
      public int CreatedBy { get; set; } // Thêm: ID của người tạo kế hoạch
      public User? Creator { get; set; } // Thêm: Thuộc tính điều hướng tới User
      [DataType(DataType.DateTime)]
      public DateTime? CreatedDate { get; set; } = DateTime.UtcNow; // Thêm: Thời gian tạo (tùy chọn)
      public ICollection<UserPlan> UserPlans { get; set; }
      public ICollection<PlanCell> PlanCells { get; set; }
    }
  }
