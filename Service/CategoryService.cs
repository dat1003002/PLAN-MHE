using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public class CategoryService : ICategoryService
  {
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddCategoryAsync(Category category)
    {
      _context.Categories.Add(category);
      await _context.SaveChangesAsync();
    }

    public async Task<bool> CategoryHasProductsAsync(int categoryId)
    {
      // Kiểm tra xem danh mục có sản phẩm hay không
      return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
      return await _context.Categories.ToListAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
      // Kiểm tra xem danh mục có sản phẩm không
      if (await CategoryHasProductsAsync(id))
      {
        throw new InvalidOperationException("Danh mục này không thể xóa vì còn chứa tiêu chuân.");
      }

      var category = await _context.Categories.FindAsync(id);
      if (category != null)
      {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
      return await _context.Categories.FindAsync(id);
    }

    public async Task UpdateCategoryAsync(Category category)
    {
      _context.Categories.Update(category);
      await _context.SaveChangesAsync();
    }

  }
}
