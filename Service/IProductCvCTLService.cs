using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductCvCTLService
  {
    Task<List<ProductCTLDTO>> GetProductsByCategoryIdAsync(int categoryId);
    Task<List<Category>> GetCategoriesAsync();
    Task CreateProductAsync(ProductCTLDTO product);
    Task DeleteProductAsync(int ProductId);
    //Task<List<ProductCTLDTO>> SearchProductsByNameAsync(string name);
    Task<List<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<ProductCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductCTLDTO product);

  }
}
