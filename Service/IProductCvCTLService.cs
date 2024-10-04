using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductCvCTLService
  {
    Task CreateProductAsync(ProductCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IPagedList<ProductCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int ProductId);
    Task<ProductCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductCTLDTO product);
    Task<IEnumerable<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);

  }
}
