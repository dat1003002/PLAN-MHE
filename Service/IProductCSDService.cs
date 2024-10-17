using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;


namespace AspnetCoreMvcFull.Service
{
  public interface IProductCSDService
  {
    Task CreateProductAsync(ProductCSDDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IEnumerable<ProductCSDDTO>> GetProducts(int categoryId);
    Task<ProductCSDDTO> GetProductByIdAsync(int productId);
    Task UpdateProductCSDAsync(ProductCSDDTO productCSDDTO);
    Task DeleteProductAsync(int ProductId);
  }
}
