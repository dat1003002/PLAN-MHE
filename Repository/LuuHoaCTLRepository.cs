using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AspnetCoreMvcFull.Repository
{
  public class LuuHoaCTLRepository : ILuuHoaCTLRepository
  {
    private readonly ApplicationDbContext _luuhoaCTLRepository;
    public LuuHoaCTLRepository(ApplicationDbContext luuhoaCTLRepository)
    {
      _luuhoaCTLRepository = luuhoaCTLRepository;
    }

    public async Task AddProductAsync(LuuHoaCTLDTO luuHoaCTLDTO)
    {
      var addProductsLHCTL = new Product
      {
        name = luuHoaCTLDTO.name,
        image = luuHoaCTLDTO.image,
        CategoryId = luuHoaCTLDTO.CategoryId,
      };
      await _luuhoaCTLRepository.Products.AddAsync(addProductsLHCTL);
      await _luuhoaCTLRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _luuhoaCTLRepository.Categories.ToListAsync();
    }
    public async Task<IQueryable<LuuHoaCTLDTO>> GetProducts(int categoryId)
    {
      var products = _luuhoaCTLRepository.Products
        .Where(p => p.CategoryId == categoryId)
        .Select(p => new LuuHoaCTLDTO
        {
          ProductId = p.ProductId,
          name = p.name,
          image = p.image,
          CategoryId = p.CategoryId,
        });
      return await Task.FromResult(products);
    }
    public async Task DeleteProductAsync(int ProductId)
    {
      var product = await _luuhoaCTLRepository.Products.FindAsync(ProductId);
      if (product != null)
      {
        _luuhoaCTLRepository.Products.Remove(product);
        await _luuhoaCTLRepository.SaveChangesAsync();
      }
    }
    public async Task<LuuHoaCTLDTO> GetProductByIdAsync(int productId)
    {
      return await _luuhoaCTLRepository.Products
         .Where(p => p.ProductId == productId)
         .Select(p => new LuuHoaCTLDTO
         {
           ProductId = p.ProductId,
           name = p.name,
           image = p.image,
           CategoryId = p.CategoryId,
         }).FirstOrDefaultAsync();
    }
    public async Task UpdateProductAsync(LuuHoaCTLDTO product)
    {
      var editProduct = await _luuhoaCTLRepository.Products.FindAsync(product.ProductId);
      if (editProduct != null)
      {
        editProduct.name = product.name;
        editProduct.image = product.image;
        editProduct.CategoryId = product.CategoryId;
        await _luuhoaCTLRepository.SaveChangesAsync();
      }
    }
    public Task<IQueryable<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      var products = _luuhoaCTLRepository.Products
         .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
         .Select(p => new LuuHoaCTLDTO
         {
           ProductId = p.ProductId,
           name = p.name,
           image = p.image,
           CategoryId = p.CategoryId
         });
      return Task.FromResult(products.AsQueryable());
    }
    public async Task<IEnumerable<LuuHoaCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var prodductTHCTL = await _luuhoaCTLRepository.Products
     .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
     .Skip((page - 1) * pageSize)
     .Take(pageSize)
     .Select(p => new LuuHoaCTLDTO
     {
       ProductId = p.ProductId,
       name = p.name,
       image = p.image,
       CategoryId = p.CategoryId
     }).ToListAsync();
      return prodductTHCTL;
    }
    
  }
}
