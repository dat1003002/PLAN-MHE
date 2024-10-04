using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Service
{
  public interface ICategoryService
  {
    Task AddCategoryAsync(Category category);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task DeleteCategoryAsync(int id);
    Task<bool> CategoryHasProductsAsync(int categoryId);
    Task<Category> GetCategoryByIdAsync(int id);
    Task UpdateCategoryAsync(Category category);

  }
}
