using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public class XuathangSevice : IXuathangSevice
  {
    private readonly IXuathangkhoRepository _xuathangkhoRepository;

    public XuathangSevice(IXuathangkhoRepository xuathangkhoRepository)
    {
      _xuathangkhoRepository = xuathangkhoRepository;
    }

    public async Task AddProductAsync(XuathangDTO product)
    {
      await _xuathangkhoRepository.AddProductAsync(product);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
      return await _xuathangkhoRepository.GetCategoriesAsync();
    }

    public async Task<IPagedList<XuathangDTO>> GetProductsAsync(int categoryId, int pageNumber, int pageSize)
    {
      var products = await _xuathangkhoRepository.GetProductsAsync(categoryId);
      return await products.ToPagedListAsync(pageNumber, pageSize);
    }

    public async Task DeleteProductAsync(int productId)
    {
      await _xuathangkhoRepository.DeleteProductAsync(productId);
    }

    public async Task<XuathangDTO> GetProductByIdAsync(int productId)
    {
      return await _xuathangkhoRepository.GetProductByIdAsync(productId);
    }

    public async Task UpdateProductAsync(XuathangDTO product)
    {
      await _xuathangkhoRepository.UpdateProductAsync(product);
    }

    public async Task<IEnumerable<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _xuathangkhoRepository.SearchProductsByNameAsync(name, categoryId);
    }

    public async Task<IPagedList<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _xuathangkhoRepository.SearchProductsByNameAsync(name, categoryId, page, pageSize);
      return await products.ToPagedListAsync(page, pageSize);
    }
  }
}
