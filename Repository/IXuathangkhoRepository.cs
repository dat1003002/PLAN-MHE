using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Repository
{
  public interface IXuathangkhoRepository
  {
    Task AddProductAsync(XuathangDTO xuathangDTO);
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<IEnumerable<XuathangDTO>> GetProductsAsync(int categoryId);
    Task DeleteProductAsync(int productId);
    Task<XuathangDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(XuathangDTO xuathangDTO);
    Task<IEnumerable<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IEnumerable<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
