namespace AspnetCoreMvcFull.ModelDTO.Product
{
  public class LoiThepCTLDTO
  {
    public int ProductId { get; set; }
    public string? mahangctl { get; set; }
    public int? mahang { get; set; }
    public string? name { get; set; }
    public string? khoangcach2daumoinoiloithep { get; set; }
    public string? khocaosubo { get; set; }
    public string? khocaosuketdinh3t { get; set; }
    public string? kholoithep { get; set; }
    public string? kichthuoccuacaosudanmoinoi { get; set; }
    public string? solink { get; set; }
    public string? sosoiloithep { get; set; }
    public string? tocdoquan { get; set; }
    public string? trongluongloithepspinning { get; set; }
    public string? dodaycaosubo { get; set; }
    public string? dodaycaosuketdinh3t { get; set; }
    public string? chieudailoithep { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int CategoryId { get; set; }
  }
}

