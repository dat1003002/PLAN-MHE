using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using X.PagedList;

namespace AspnetCoreMvcFull.Service
{
  public class LuuHoaCTLSevice : ILuuHoaCTLSevice
  {
    private readonly ILuuHoaCTLRepository _luuHoaCTLRepository;
    public LuuHoaCTLSevice(ILuuHoaCTLRepository luuHoaCTLRepository)
    {
      _luuHoaCTLRepository = luuHoaCTLRepository;
    }

    public async Task AddProductAsync(LuuHoaCTLDTO luuHoaCTLDTO)
    {
      await _luuHoaCTLRepository.AddProductAsync(luuHoaCTLDTO);
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      await _luuHoaCTLRepository.DeleteProductAsync(ProductId);
    }

    public Task<IEnumerable<Category>> GetCategories()
    {
      return _luuHoaCTLRepository.GetCategories();
    }

    public async Task<LuuHoaCTLDTO> GetProductByIdAsync(int productId)
    {
      return await _luuHoaCTLRepository.GetProductByIdAsync(productId);
    }

    public async Task<IPagedList<LuuHoaCTLDTO>> GetProducts(int categoryId, int pageNumber, int pageSize)
    {
      var products = await _luuHoaCTLRepository.GetProducts(categoryId);
      return await products.ToPagedListAsync(pageNumber, pageSize);
    }

    public async Task<IEnumerable<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _luuHoaCTLRepository.SearchProductsByNameAsync(name, categoryId);
    }

    public async Task<IPagedList<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _luuHoaCTLRepository.SearchProductsByNameAsync(name, categoryId);
      return await products.ToPagedListAsync(page, pageSize);
    }

    public async Task UpdateProductAsync(LuuHoaCTLDTO product)
    {
      await _luuHoaCTLRepository.UpdateProductAsync(product);
    }
  }
}
