using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public class GangCauCTLService : IGangCauCTLService
  {
    private readonly IGangCauCTLRepository _gangCauCTLRepository;
    public GangCauCTLService(IGangCauCTLRepository gangCauCTLRepository)
    {
      _gangCauCTLRepository = gangCauCTLRepository;
    }

    public async Task AddProductAsync(ProductGCCTLDTO product)
    {
     await _gangCauCTLRepository.AddProductAsync(product);
    }
    public async Task DeleteProductAsync(int ProductId)
    {
      await _gangCauCTLRepository.DeleteProductAsync(ProductId);
    }
    public Task<IEnumerable<Category>> GetCategories()
    {
      return _gangCauCTLRepository.GetCategories();
    }
    public async Task<ProductGCCTLDTO> GetProductByIdAsync(int productId)
    {
     return await _gangCauCTLRepository.GetProductByIdAsync(productId);
    }
    public async Task<IPagedList<ProductGCCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize)
    {
      var products = await _gangCauCTLRepository.GetProducts(categoryId);
      return await products.ToPagedListAsync(pageNumber, pageSize);
    }
    public async Task<IEnumerable<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _gangCauCTLRepository.SearchProductsByNameAsync(name, categoryId);
    }
    public async Task<IPagedList<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _gangCauCTLRepository.SearchProductsByNameAsync(name,categoryId);
      return await products.ToPagedListAsync(page, pageSize);
    }
    public async Task UpdateProductAsync(ProductGCCTLDTO product)
    {
      await _gangCauCTLRepository.UpdateProductAsync(product);
    }
  }
}
