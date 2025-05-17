using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public interface IProductLTCTLService
  {
    Task CreateProductAsync(LoiThepCTLDTO product);
    Task<IEnumerable<Category>> GetCategories();
    Task<IEnumerable<LoiThepCTLDTO>> GetProducts(int categoryId);
    Task<LoiThepCTLDTO> GetProductByIdAsync(int productId);
    Task UpdateProductAsync(LoiThepCTLDTO loiThepCTLDTO);
    Task DeleteProductAsync(int ProductId);
  }
}
