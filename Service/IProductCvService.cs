using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductCvService
  {
    Task AddProductAsync(ProductDTO productDTO);
    Task<IEnumerable<Category>> GetCategories();
    Task<IPagedList<ProductDTO>> GetProducts(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int ProductId);
    Task<ProductDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductDTO productDTO);
    Task<IEnumerable<ProductDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<ProductDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
