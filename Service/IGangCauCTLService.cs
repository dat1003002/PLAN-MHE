using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public interface IGangCauCTLService
  {
    Task AddProductAsync(ProductGCCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IPagedList<ProductGCCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int ProductId);
    Task<ProductGCCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductGCCTLDTO product);
    Task<IEnumerable<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
