using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductCvGCService
  {
    Task AddProductAsync(ProductGCMHEDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IPagedList<ProductGCMHEDTO>> GetProducts(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int ProductId);
    Task<ProductGCMHEDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductGCMHEDTO product);
    Task<IEnumerable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
