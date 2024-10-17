namespace AspnetCoreMvcFull.ModelDTO.Product
{
  public class ProductCSCTLDTO
  {
    public int ProductId { get; set; }
    public string? name { get; set; }
    public string? image { get; set; }
    public IFormFile? imageFile { get; set; }
    public int CategoryId { get; set; }
  }
}
