using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public class ProductCvService : IProductCvService
  {
    private readonly IProductCvRepository _productCvRepository;

    public ProductCvService(IProductCvRepository productCvRepository)
    {
      _productCvRepository = productCvRepository;
    }

    public async Task AddProductAsync(ProductDTO productDTO)
    {
      await _productCvRepository.AddProductAsync(productDTO);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productCvRepository.GetCategories();
    }

    public async Task<IEnumerable<ProductDTO>> GetProducts(int categoryId)
    {
      return await _productCvRepository.GetProducts(categoryId);
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      await _productCvRepository.DeleteProductAsync(ProductId);
    }

    public async Task<ProductDTO> GetProductByIdAsync(int productId)
    {
      return await _productCvRepository.GetProductByIdAsync(productId);
    }

    public async Task UpdateProductAsync(ProductDTO productDTO)
    {
      await _productCvRepository.UpdateProductAsync(productDTO);
    }
    public async Task<IEnumerable<ProductDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _productCvRepository.SearchProductsByNameAsync(name, categoryId);
    }
  }
}
