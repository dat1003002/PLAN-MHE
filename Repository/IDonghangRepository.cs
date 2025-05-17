using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  public interface IDonghangRepository
  {
    Task AddProductAsync(DonghangkhoDTO donghangkhoDTO);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<DonghangkhoDTO>> GetProducts(int categoryId);
    Task DeleteProductAsync(int ProductId);
    Task<DonghangkhoDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(DonghangkhoDTO donghangkhoDTO);
    Task<IQueryable<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId);
    Task<IEnumerable<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize);
  }
}
