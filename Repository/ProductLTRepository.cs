using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repositories
{
  public class ProductLTRepository : IProductLTRepository
  {
    private readonly ApplicationDbContext _context;

    public ProductLTRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddProductAsync(Product product)
    {
      _context.Products.Add(product); // Thêm sản phẩm mới vào cơ sở dữ liệu
      await _context.SaveChangesAsync(); // Lưu thay đổi
    }

    public async Task DeleteProductAsync(int productId)
    {
      var product = await _context.Products.FindAsync(productId); // Tìm sản phẩm theo ID
      if (product != null)
      {
        _context.Products.Remove(product); // Xóa sản phẩm khỏi cơ sở dữ liệu
        await _context.SaveChangesAsync(); // Lưu thay đổi
      }
    }

    public  async Task<IEnumerable<Category>> GetCategories()
    {
      return await _context.Categories.ToListAsync(); // Lấy tất cả danh mục
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
      return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
    {
      return await _context.Products
          .Where(p => p.CategoryId == categoryId) // Lọc sản phẩm theo ID danh mục
          .ToListAsync(); // Lấy danh sách sản phẩm
    }

    public async Task UpdateProductAsync(Product product)
    {
      _context.Products.Update(product);
      await _context.SaveChangesAsync();
    }
  }
}
