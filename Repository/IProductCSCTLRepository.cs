using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Repository
{
  public interface IProductCSCTLRepository
  {
    Task AddProductAsync(ProductCSCTLDTO productCSCTLDTO);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<ProductCSCTLDTO>> GetProducts(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<ProductCSCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductCSCTLDTO product);
    Task<IQueryable<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IEnumerable<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
