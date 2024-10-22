using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;

namespace AspnetCoreMvcFull.Repository
{
  public interface IGangCauCTLRepository
  {
    Task AddProductAsync(ProductGCCTLDTO productGCCTLDTO);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<ProductGCCTLDTO>> GetProducts(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<ProductGCCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductGCCTLDTO product);
    Task<IQueryable<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IEnumerable<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
