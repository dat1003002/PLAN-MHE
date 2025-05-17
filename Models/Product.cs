using System.ComponentModel.DataAnnotations.Schema;

namespace AspnetCoreMvcFull.Models
{
  public class Product
  {
    public int ProductId { get; set; }
    public int? mahang { get; set; }
    public string? mahangctl { get; set; }
    public string? name { get; set; }
    public string? image { get; set; }
    public string? quycachloithep { get; set; }
    public string? khuonlodie { get; set; }
    public string? khuonsoiholder { get; set; }
    public int? sosoi { get; set; }
    public string? pitch { get; set; }
    public string? tieuchuan { get; set; }
    public string? thucte { get; set; }
    public string? doday { get; set; }
    public string? soi1 { get; set; }
    public string? soi2 { get; set; }
    public string? sodaycatduoc { get; set; }
    public string? chieudaicatlon { get; set; }
    public string? chieudaicatnho { get; set; }
    public string? tocdomaydun { get; set; }
    public string? tocdokeo { get; set; }
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
    public Category Category { get; set; }
    public string? chieudailoithep { get; set; }
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
    public string? aplucdaudunloithep { get; set; }
    public string? caosubo { get; set; }
    public string? caosuketdinh { get; set; }
    public string? caosur514 { get; set; }
    public string? khoangcach2daumoinoibo { get; set; }
    public string? loithepsaukhidun { get; set; }
    public string? loitheptruockhidun { get; set; }
    public string? nhietdodaumaydun { get; set; }
    public string? nhietdotrucxoan { get; set; }
    public string? tocdocolingdrum { get; set; }
    public string? tocdoduncaosu { get; set; }
    public string? tocdodun { get; set; }
    public string? loaicaosu { get; set; }
    public string? trongluogtest { get; set; }
    public string? chieudai { get; set; }
    public string? chieudai1 { get; set; }
    public string? chieudai2 { get; set; }
    public string? doday1 { get; set; }
    public string? doday2 { get; set; }
    public string? kho { get; set; }
    public string? kho1 { get; set; }
    public string? kho2 { get; set; }
    public string? loaicaosu1 { get; set; }
    public string? loaicaosu2 { get; set; }
    public string? loaikhuondun { get; set; }
    public string? loaikhuondun1 { get; set; }
    public string? loaikhuondun2 { get; set; }
    public string? tocdomotor { get; set; }
    public string? tocdomotor1 { get; set; }
    public string? tocdomotor2 { get; set; }
    public string? trongluong { get; set; }
    public string? trongluong1 { get; set; }
    public string? trongluong2 { get; set; }
  }
}
