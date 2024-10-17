using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductCSCTLService
  {
    Task AddProductAsync(ProductCSCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IPagedList<ProductCSCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int ProductId);
    Task<ProductCSCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductCSCTLDTO product);
    Task<IEnumerable<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
