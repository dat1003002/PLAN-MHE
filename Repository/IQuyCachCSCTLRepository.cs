using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Repository
{
  public interface IQuyCachCSCTLRepository
  {
    Task CreateProductAsync(QuyCachCaoSuCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<QuyCachCaoSuCTLDTO>> GetProducts(int categoryId);
    Task<QuyCachCaoSuCTLDTO> GetProductByIdAsync(int productId);
    Task DeleteProductAsync(int ProductId);
    Task UpdateProductAsync(QuyCachCaoSuCTLDTO quyCachCaoSuCTLDTO);
  }
}
