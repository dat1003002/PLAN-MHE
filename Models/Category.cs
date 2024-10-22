namespace AspnetCoreMvcFull.Models
{
  public class Category
  {
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public string Tenxuong {  get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime LastUpdated => UpdatedAt ?? CreatedAt;
    // Thuộc tính điều hướng một-nhiều
    public ICollection<Product> Products { get; set; } = new List<Product>();
  }
}
