using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PLANMHE.Models
{
  public class UserType
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên loại người dùng là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên loại người dùng không được vượt quá 50 ký tự")]
    public string FullName { get; set; }

    public string? Note { get; set; } // Cho phép null, không thay đổi

    public ICollection<User> Users { get; set; }

    public UserType()
    {
      Users = new List<User>();
    }
  }
}
