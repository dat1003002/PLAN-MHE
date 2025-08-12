using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PLANMHE.Models
{
  public class User
  {
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
    public string FullName { get; set; }
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
    public string Username { get; set; }
    public string Password { get; set; } // Lưu ý: Băm mật khẩu trong UserService
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? Phone { get; set; }
    public string? Address { get; set; }
    [Required(ErrorMessage = "Giới tính là bắt buộc")]
    public string Sex { get; set; }
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Loại người dùng là bắt buộc")]
    public int UserTypeId { get; set; } // Đã có [Required] trong code gốc, giữ nguyên
    public UserType? UserType { get; set; } // Thuộc tính điều hướng, tùy chọn
    public ICollection<UserPlan> UserPlans { get; set; }
    public User()
    {
      UserPlans = new List<UserPlan>();
    }
  }
}
