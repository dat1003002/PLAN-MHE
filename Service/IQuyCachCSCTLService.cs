using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Service
{
  public interface IQuyCachCSCTLService
  {
    Task CreateProductAsync(QuyCachCaoSuCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IEnumerable<QuyCachCaoSuCTLDTO>> GetProducts(int categoryId);
    Task<QuyCachCaoSuCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(QuyCachCaoSuCTLDTO quyCachCaoSuCTLDTO);
    Task DeleteProductAsync(int ProductId);
  }
}
