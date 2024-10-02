using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductCvService
  {
    Task AddProductAsync(ProductDTO productDTO); // Thêm sản phẩm
    Task<IEnumerable<Category>> GetCategories(); // Lấy danh mục
    Task<IEnumerable<ProductDTO>> GetProducts(int categoryId); // Lấy sản phẩm theo categoryId
    Task DeleteProductAsync(int ProductId);
    Task<ProductDTO> GetProductByIdAsync(int productId); // Lấy sản phẩm theo id
    Task UpdateProductAsync(ProductDTO productDTO);
    Task<IEnumerable<ProductDTO>> SearchProductsByNameAsync(string name, int categoryId);
  }
}
