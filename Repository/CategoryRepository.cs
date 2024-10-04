using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repositories
{
  public class CategoryRepository : ICategoryRepository
  {
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddCategoryAsync(Category category)
    {
      await _context.Categories.AddAsync(category);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
      var category = await _context.Categories.FindAsync(id);
      if (category != null)
      {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
      }
    }

     public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
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
