using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using X.PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public class ProductCSDService : IProductCSDService
  {
    private readonly IProductCSDRepository _productCSDRepository;

    public ProductCSDService(IProductCSDRepository productCSDRepository)
    {
      _productCSDRepository = productCSDRepository;
    }

    public async Task CreateProductAsync(ProductCSDDTO product)
    {
      await _productCSDRepository.CreateProductAsync(product);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productCSDRepository.GetCategories();
    }

    public async Task<IEnumerable<ProductCSDDTO>> GetProducts(int categoryId)
    {
      var products = await _productCSDRepository.GetProducts(categoryId);
      return products.ToList();
    }

    public async Task<ProductCSDDTO> GetProductByIdAsync(int productId)
    {
      return await _productCSDRepository.GetProductByIdAsync(productId);
    }

    public async Task UpdateProductCSDAsync(ProductCSDDTO productCSDDTO)
    {
      await _productCSDRepository.UpdateProducCSDAsync(productCSDDTO);
    }

    public async Task DeleteProductAsync(int productId)
    {
      await _productCSDRepository.DeleteProductAsync(productId);
    }
  }
}
