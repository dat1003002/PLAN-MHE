using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  public interface IProductLTCTLRepository
  {
    Task CreateProductAsync(LoiThepCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IQueryable<LoiThepCTLDTO>> GetProducts(int categoryId);
    Task<LoiThepCTLDTO> GetProductByIdAsync(int productId);
    Task DeleteProductAsync(int ProductId);
    Task UpdateProductAsync(LoiThepCTLDTO loiThepCTLDTO);
  }
}
