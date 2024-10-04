using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Repository
{
  public interface IProductCvGCRepository
  {
    Task AddProductAsync(ProductGCMHEDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<ProductGCMHEDTO>> GetProducts(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<ProductGCMHEDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductGCMHEDTO product);
    Task<IQueryable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IEnumerable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
