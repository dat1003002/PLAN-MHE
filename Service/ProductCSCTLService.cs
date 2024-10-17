using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public class ProductCSCTLService : IProductCSCTLService
  {
    private readonly IProductCSCTLRepository _productCSCTLRepository;

    public ProductCSCTLService(IProductCSCTLRepository productCSCTLRepository)
    {
      _productCSCTLRepository = productCSCTLRepository;
    }

    public async Task AddProductAsync(ProductCSCTLDTO product)
    {
      await _productCSCTLRepository.AddProductAsync(product);
    }

    public async Task DeleteProductAsync(int productId)
    {
      await _productCSCTLRepository.DeleteProductAsync(productId);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productCSCTLRepository.GetCategories();
    }

    public async Task<ProductCSCTLDTO> GetProductByIdAsync(int productId)
    {
      return await _productCSCTLRepository.GetProductByIdAsync(productId);
    }

    public async Task<IPagedList<ProductCSCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize)
    {
      var products = await _productCSCTLRepository.GetProducts(categoryId);
      return await products.ToPagedListAsync(pageNumber, pageSize);
    }

    public async Task<IEnumerable<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _productCSCTLRepository.SearchProductsByNameAsync(name, categoryId);
    }

    public async Task<IPagedList<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _productCSCTLRepository.SearchProductsByNameAsync(name, categoryId);
      return await products.ToPagedListAsync(page, pageSize);
    }

    public async Task UpdateProductAsync(ProductCSCTLDTO product)
    {
      await _productCSCTLRepository.UpdateProductAsync(product);
    }
  }
}
