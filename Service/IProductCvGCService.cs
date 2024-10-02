using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductCvGCService
  {
    Task<IEnumerable<ProductGCMHEDTO>> GetProductsByCategoryAsync(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task AddProductAsync(ProductGCMHEDTO product);
    Task<IEnumerable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task UpdateProductAsync(ProductGCMHEDTO product);
    Task<ProductGCMHEDTO> GetProductByIdAsync(int id);
  }
}
