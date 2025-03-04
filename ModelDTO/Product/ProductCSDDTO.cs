namespace AspnetCoreMvcFull.ModelDTO.Product
{
  public class ProductCSDDTO
  {
    public int ProductId { get; set; }
    public int? mahang { get; set; }
    public string? name { get; set; }
    public string? may { get; set; }
    public string? solinkthanchinh { get; set; }
    public string? solinkthannoi { get; set; }
    public string? caosuloplot { get; set; }
    public string? caosubemat { get; set; }
    public string? docoloplot { get; set; }
    public string? docobemat { get; set; }
    public string? khuondunloplot { get; set; }
    public string? khuondunbemat { get; set; }
    public string? khotieuchuanloplot { get; set; }
    public string? khotieuchuanbemat { get; set; }
    public string? chieudaithanchinhloplot { get; set; }
    public string? chieudaithanchinhbemat { get; set; }
    public string? chieudaithannoiloplot { get; set; }
    public string? chieudaithannoibemat { get; set; }
    public string? trongluongthanchinhloplot { get; set; }
    public string? trongluongthanchinhbemat { get; set; }
    public string? trongluongthannoiloplot { get; set; }
    public string? trongluongthannoibemat { get; set; }
    public string? trongluongdaukibemat { get; set; }
    public string? trongluongdaukiloplot { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int CategoryId { get; set; }
  }
}
