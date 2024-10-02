using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  public interface IProductCvRepository
  {
    Task AddProductAsync(ProductDTO productDTO);
    Task<IEnumerable<Category>> GetCategories();
    Task<IEnumerable<ProductDTO>> GetProducts(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<ProductDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(ProductDTO productDTO);
    Task<IEnumerable<ProductDTO>> SearchProductsByNameAsync(string name, int categoryId);
  }
}
