using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using AspnetCoreMvcFull.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Services
{
  // Triển khai IProductLTService để xử lý các yêu cầu dịch vụ cho ProductLT
  public class ProductLTService : IProductLTService
  {
    private readonly IProductLTRepository _productLTRepository;

    public ProductLTService(IProductLTRepository productRepository)
    {
      _productLTRepository = productRepository;
    }

    public async Task AddProductAsync(Product product)
    {
      await _productLTRepository.AddProductAsync(product); // Thêm sản phẩm mới thông qua repository
    }

    public async Task DeleteProductAsync(int productId)
    {
      await _productLTRepository.DeleteProductAsync(productId); // Xóa sản phẩm thông qua repository
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productLTRepository.GetCategories(); // Lấy tất cả danh mục thông qua repository
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
      return await _productLTRepository.GetProductByIdAsync(id);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
    {
      return await _productLTRepository.GetProductsByCategoryIdAsync(categoryId); // Lấy sản phẩm theo ID danh mục thông qua repository
    }
    public async Task UpdateProductAsync(Product product)
    {
      await _productLTRepository.UpdateProductAsync(product);
    }
  }
}
