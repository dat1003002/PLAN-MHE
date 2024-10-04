namespace AspnetCoreMvcFull.Models
{
  public class Pagination
  {
    public int Id { get; set; }
    public int TotalItems { get; set; } // Tổng số mục
    public int ItemsPerPage { get; set; } // Số mục trên mỗi trang
    public int CurrentPage { get; set; } // Trang hiện tại
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
  }
}
