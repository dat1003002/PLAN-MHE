using System.ComponentModel.DataAnnotations;

namespace PLANMHE.Models
{
  public class UserPlan
  {
    [Required]
    public int UserId { get; set; }

    [Required]
    public int PlanId { get; set; }

    public User User { get; set; }
    public Plan Plan { get; set; }
  }
}
