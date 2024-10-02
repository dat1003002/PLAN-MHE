using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;

namespace AspnetCoreMvcFull.Service
{
  public class ProductCvCTLService : IProductCvCTLService
  {
    private readonly IProductCvCTLRepository _productCvCTLRepository;

    public ProductCvCTLService(IProductCvCTLRepository productCvCTLRepository)
    {
      _productCvCTLRepository = productCvCTLRepository;
    }

    public async Task CreateProductAsync(ProductCTLDTO product)
    {
      await _productCvCTLRepository.CreateProductAsync(product);
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      await _productCvCTLRepository.DeleteProductAsync(ProductId);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
      return await _productCvCTLRepository.GetCategoriesAsync();
    }

    public async Task<ProductCTLDTO> GetProductByIdAsync(int productId)
    {
      return await _productCvCTLRepository.GetProductByIdAsync(productId);
    }

    public async Task<List<ProductCTLDTO>> GetProductsByCategoryIdAsync(int categoryId)
    {
      return await _productCvCTLRepository.GetProductsByCategoryIdAsync(categoryId);
    }

    public async Task<List<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _productCvCTLRepository.SearchProductsByNameAsync(name, categoryId);
    }

    //public async Task<List<ProductCTLDTO>> SearchProductsByNameAsync(string name)
    //{
    //  return await _productCvCTLRepository.SearchProductsByNameAsync(name);
    //}

    public async Task UpdateProductAsync(ProductCTLDTO product)
    {
      await _productCvCTLRepository.UpdateProductAsync(product);
    }
  }
}
