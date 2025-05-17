using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public interface IDongHangService
  {
    Task AddProductAsync(DonghangkhoDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IPagedList<DonghangkhoDTO>> GetProducts(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int ProductId);
    Task<DonghangkhoDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(DonghangkhoDTO product);
    Task<IEnumerable<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
