using AspnetCoreMvcFull.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  // Interface định nghĩa các phương thức truy cập dữ liệu cho ProductLT
  public interface IProductLTRepository
  {
    Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId); // Lấy sản phẩm theo ID danh mục
    Task AddProductAsync(Product product); // Thêm sản phẩm mới
    Task<IEnumerable<Category>> GetCategories(); // Lấy tất cả danh mục
    Task DeleteProductAsync(int productId); // Xóa sản phẩm theo ID
    Task<Product> GetProductByIdAsync(int id); // Lấy sản phẩm theo ID
    Task UpdateProductAsync(Product product); // Cập nhật sản phẩm
    Task<IEnumerable<Product>> SearchProductsByNameAsync(string name);

  }
}
