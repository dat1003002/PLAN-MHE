using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Repository
{
  public interface IProductCvGCRepository
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
