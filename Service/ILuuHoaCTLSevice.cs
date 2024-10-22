using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public interface ILuuHoaCTLSevice
  {
    Task AddProductAsync(LuuHoaCTLDTO luuHoaCTLDTO);
    Task<IEnumerable<Category>> GetCategories();
    Task<IPagedList<LuuHoaCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize);
    Task DeleteProductAsync(int ProductId);
    Task<LuuHoaCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(LuuHoaCTLDTO product);
    Task<IEnumerable<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IPagedList<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
