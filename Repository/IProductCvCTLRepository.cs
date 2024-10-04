using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Repository
{
  public interface IProductCvCTLRepository
  {
    Task CreateProductAsync(ProductCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<ProductCTLDTO>> GetProducts(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<ProductCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductCTLDTO product);
    Task<IQueryable<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IEnumerable<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);

  }
}
