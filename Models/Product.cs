namespace AspnetCoreMvcFull.Models
{
  public class Product
  {
    public int ProductId { get; set; }
    public int? mahang { get; set; }
    public string? name { get; set; }
    public string? image {  get; set; }
    public string? quycachloithep { get; set; }
    public string? khuonlodie { get; set; }
    public string? khuonsoiholder { get; set; }
    public int? sosoi { get; set;}
    public string? pitch { get; set; }
    public string? tieuchuan { get; set; }
    public string? thucte { get; set; }
    public string? doday {  get; set; }
    public string? soi1 { get; set; }
    public string? soi2 { get; set; }
    public string? sodaycatduoc { get; set; }
    public string? chieudaicatlon {  get; set; }
    public string? chieudaicatnho { get; set; }
    public  string? tocdomaydun {  get; set; }
    public string? tocdokeo { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }

  }
}
