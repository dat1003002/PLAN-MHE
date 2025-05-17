using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public interface IXuathangSevice
  {
    Task AddProductAsync(XuathangDTO product);
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<IPagedList<XuathangDTO>> GetProductsAsync(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int productId);
    Task<XuathangDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(XuathangDTO product);
    Task<IEnumerable<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
