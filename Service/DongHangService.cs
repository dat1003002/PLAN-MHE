using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public class DongHangService : IDongHangService
  {
    private readonly IDonghangRepository _donghangRepository;

    public DongHangService(IDonghangRepository donghangRepository)
    {
      _donghangRepository = donghangRepository;
    }

    public async Task AddProductAsync(DonghangkhoDTO product)
    {
      await _donghangRepository.AddProductAsync(product);
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      await _donghangRepository.DeleteProductAsync(ProductId);
    }

    public Task<IEnumerable<Category>> GetCategories()
    {
      return _donghangRepository.GetCategories();
    }

    public async Task<DonghangkhoDTO> GetProductByIdAsync(int productId)
    {
      return await _donghangRepository.GetProductByIdAsync(productId);
    }

    public async Task<IPagedList<DonghangkhoDTO>> GetProducts(int categoryId, int pageNumber, int pageSize)
    {
      var products = await _donghangRepository.GetProducts(categoryId);
      return await products.ToPagedListAsync(pageNumber, pageSize);
    }

    public async Task<IEnumerable<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      var products = await _donghangRepository.SearchProductsByNameAsync(name, categoryId);
      return await products.ToListAsync();
    }

    public async Task<IPagedList<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _donghangRepository.SearchProductsByNameAsync(name, categoryId);
      return await products.ToPagedListAsync(page, pageSize);
    }

    public async Task UpdateProductAsync(DonghangkhoDTO product)
    {
      await _donghangRepository.UpdateProductAsync(product);
    }
  }
}
