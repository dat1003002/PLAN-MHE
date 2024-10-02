using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;

namespace AspnetCoreMvcFull.Service
{
  public class ProductCvGCService : IProductCvGCService
  {
    private readonly IProductCvGCRepository _productRepository;
    public ProductCvGCService(IProductCvGCRepository productRepository)
    {
      _productRepository = productRepository;
    }

    public async Task AddProductAsync(ProductGCMHEDTO product)
    {
      await _productRepository.AddProductAsync(product);
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      await _productRepository.DeleteProductAsync(ProductId);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
      return await _productRepository.GetCategoriesAsync();
    }

    public async Task<ProductGCMHEDTO> GetProductByIdAsync(int id)
    {
      return await _productRepository.GetProductByIdAsync(id);
    }

    public async Task<IEnumerable<ProductGCMHEDTO>> GetProductsByCategoryAsync(int categoryId)
    {
      return await _productRepository.GetProductsByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _productRepository.SearchProductsByNameAsync(name, categoryId);
    }

    public async Task UpdateProductAsync(ProductGCMHEDTO product)
    {
      await _productRepository.UpdateProductAsync(product);
    }
  }
}
