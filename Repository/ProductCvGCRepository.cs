using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  public class ProductCvGCRepository : IProductCvGCRepository
  {
    private readonly ApplicationDbContext _DBGCcontext;
    public ProductCvGCRepository(ApplicationDbContext dbGCContext)
    {
      _DBGCcontext = dbGCContext;
    }

    public async Task AddProductAsync(ProductGCMHEDTO product)
    {
      var newProduct = new Product
      {
        name = product.name,
        image = product.image,
        CategoryId = product.CategoryId
      };

      await _DBGCcontext.Products.AddAsync(newProduct);
      await _DBGCcontext.SaveChangesAsync();
    }
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _DBGCcontext.Categories.ToListAsync();
    }
    public async Task<IQueryable<ProductGCMHEDTO>> GetProducts(int categoryId)
    {
      var products = _DBGCcontext.Products
                             .Where(p => p.CategoryId == categoryId)
                             .Select(p => new ProductGCMHEDTO
                             {
                               ProductId = p.ProductId,
                               name = p.name,
                               image = p.image,
                               CategoryId = p.CategoryId
                             });

      // Trả về IQueryable trực tiếp mà không cần sử dụng Task.FromResult
      return await Task.FromResult(products);
    }
    public async Task DeleteProductAsync(int ProductId)
    {
      var product = await _DBGCcontext.Products.FindAsync(ProductId);
      if (product != null)
      {
        _DBGCcontext.Products.Remove(product);
        await _DBGCcontext.SaveChangesAsync();
      }
    }
    public async Task<ProductGCMHEDTO> GetProductByIdAsync(int productId)
    {
      return await _DBGCcontext.Products
          .Where(p => p.ProductId == productId)
          .Select(p => new ProductGCMHEDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          }).FirstOrDefaultAsync();
    }
    public async Task UpdateProductAsync(ProductGCMHEDTO product)
    {
      var existingProduct = await _DBGCcontext.Products.FindAsync(product.ProductId);
      if (existingProduct != null)
      {
        existingProduct.name = product.name;
        existingProduct.image = product.image;
        existingProduct.CategoryId = product.CategoryId;

        await _DBGCcontext.SaveChangesAsync();
      }
    }
    public async Task<IEnumerable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _DBGCcontext.Products
           .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)  
           .Skip((page - 1) * pageSize)                                       
           .Take(pageSize)                                                    
           .Select(p => new ProductGCMHEDTO
           {
             ProductId = p.ProductId,
             name = p.name,
             image = p.image,
             CategoryId = p.CategoryId
           })
           .ToListAsync();

      return products;
    }
    Task<IQueryable<ProductGCMHEDTO>> IProductCvGCRepository.SearchProductsByNameAsync(string name, int categoryId)
    {
      var products = _DBGCcontext.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
          .Select(p => new ProductGCMHEDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          });

      return Task.FromResult(products.AsQueryable());
    }
  }
}
