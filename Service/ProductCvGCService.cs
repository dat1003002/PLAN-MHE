using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public class ProductCvGCService : IProductCvGCService
  {
    private readonly IProductCvGCRepository _productRepository;
    public ProductCvGCService(IProductCvGCRepository productRepository)
    {
      _productRepository = productRepository;
    }
    public async Task AddProductAsync (ProductGCMHEDTO product)
    {
      await _productRepository.AddProductAsync(product);
    }
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productRepository.GetCategories();
    }
    public async Task<IPagedList<ProductGCMHEDTO>> GetProducts(int categoryId, int pageNumber, int pageSize)
    {
      var query = await _productRepository.GetProducts(categoryId);

      // Sau đó sử dụng ToPagedListAsync để phân trang
      return await query.ToPagedListAsync(pageNumber, pageSize);
    }
    public async Task DeleteProductAsync(int ProductId)
    {
      await _productRepository.DeleteProductAsync(ProductId);
    }
    public async Task<ProductGCMHEDTO> GetProductByIdAsync(int productId)
    {
      return await _productRepository.GetProductByIdAsync(productId);
    }
    public async Task UpdateProductAsync(ProductGCMHEDTO product)
    {
      await _productRepository.UpdateProductAsync(product);
    }
    public async Task<IEnumerable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _productRepository.SearchProductsByNameAsync(name, categoryId);
    }
    public async Task<IPagedList<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var query = await _productRepository.SearchProductsByNameAsync(name, categoryId);

      // Sử dụng ToPagedListAsync để phân trang
      return await query.ToPagedListAsync(page, pageSize);
    }
  }
}
