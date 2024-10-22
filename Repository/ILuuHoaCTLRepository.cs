using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;

namespace AspnetCoreMvcFull.Repository
{
  public interface ILuuHoaCTLRepository
  {
    Task AddProductAsync(LuuHoaCTLDTO luuHoaCTLDTO);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<LuuHoaCTLDTO>> GetProducts(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<LuuHoaCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(LuuHoaCTLDTO product);
    Task<IQueryable<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IEnumerable<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
