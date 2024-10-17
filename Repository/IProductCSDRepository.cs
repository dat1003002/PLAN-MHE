using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Repository
{
  public interface IProductCSDRepository
  {
    Task CreateProductAsync(ProductCSDDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<ProductCSDDTO>> GetProducts(int categoryId);
    Task<ProductCSDDTO> GetProductByIdAsync(int productId);
    Task DeleteProductAsync(int ProductId);
    Task UpdateProducCSDAsync(ProductCSDDTO productCSDDTO);
  }
}
