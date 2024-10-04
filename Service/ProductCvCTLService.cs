using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productCvCTLRepository.GetCategories();
    }
    public async Task<IPagedList<ProductCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize)
    {
      var query = await _productCvCTLRepository.GetProducts(categoryId);

      // Sau đó sử dụng ToPagedListAsync để phân trang
      return await query.ToPagedListAsync(pageNumber, pageSize);
    }
    public async Task DeleteProductAsync(int ProductId)
    {
      await _productCvCTLRepository.DeleteProductAsync(ProductId);
    }
    public async Task<ProductCTLDTO> GetProductByIdAsync(int productId)
    {
      return await _productCvCTLRepository.GetProductByIdAsync(productId);
    }
    public async Task UpdateProductAsync(ProductCTLDTO product)
    {
      await _productCvCTLRepository.UpdateProductAsync(product);
    }
    public async Task<IEnumerable<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
     return await _productCvCTLRepository.SearchProductsByNameAsync(name, categoryId);
    }
    public async Task<IPagedList<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var query = await _productCvCTLRepository.SearchProductsByNameAsync(name, categoryId);

      // Sử dụng ToPagedListAsync để phân trang
      return await query.ToPagedListAsync(page, pageSize);
    }
  }
}
