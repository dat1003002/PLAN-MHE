using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;

namespace AspnetCoreMvcFull.Service
{
  public class ProductLTCTLService : IProductLTCTLService
  {
    private readonly IProductLTCTLRepository _productLTCTLRepository;

    public ProductLTCTLService(IProductLTCTLRepository productLTCTLRepository)
    {
      _productLTCTLRepository = productLTCTLRepository;
    }

    public async Task CreateProductAsync(LoiThepCTLDTO product)
    {
      await _productLTCTLRepository.CreateProductAsync(product);
    }
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productLTCTLRepository.GetCategories();
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      await _productLTCTLRepository.DeleteProductAsync(ProductId);
    }
    public async Task<IEnumerable<LoiThepCTLDTO>> GetProducts(int categoryId)
    {
      var products = await _productLTCTLRepository.GetProducts(categoryId);
      return products.ToList();
    }

    public async Task<LoiThepCTLDTO> GetProductByIdAsync(int productId)
    {
      return await _productLTCTLRepository.GetProductByIdAsync(productId);
    }

    public async Task UpdateProductAsync(LoiThepCTLDTO loiThepCTLDTO)
    {
      await _productLTCTLRepository.UpdateProductAsync(loiThepCTLDTO);
    }
  }
}
