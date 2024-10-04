using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  public class ProductCvRepository : IProductCvRepository
  {
    private readonly ApplicationDbContext _context;

    public ProductCvRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddProductAsync(ProductDTO productDTO)
    {
      var product = new Product
      {
        ProductId = productDTO.ProductId,
        name = productDTO.name,
        image = productDTO.image,
        CategoryId = productDTO.CategoryId
      };

      _context.Products.Add(product);
      await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _context.Categories.ToListAsync();
    }
    public Task<IQueryable<ProductDTO>> GetProducts(int categoryId)
    {
      var products = _context.Products
                             .Where(p => p.CategoryId == categoryId)
                             .Select(p => new ProductDTO
                             {
                               ProductId = p.ProductId,
                               name = p.name,
                               image = p.image,
                               CategoryId = p.CategoryId
                             });

      // Trả về IQueryable trực tiếp mà không cần sử dụng Task.FromResult
      return Task.FromResult(products);
    }
    public async Task DeleteProductAsync(int ProductId)
    {
      var product = await _context.Products.FindAsync(ProductId);
      if (product != null)
      {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
      }
    }
    public async Task<ProductDTO> GetProductByIdAsync(int productId)
    {
      return await _context.Products
          .Where(p => p.ProductId == productId)
          .Select(p => new ProductDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          }).FirstOrDefaultAsync();
    }
    public async Task UpdateProductAsync(ProductDTO productDTO)
    {
      var product = await _context.Products.FindAsync(productDTO.ProductId);
      if (product != null)
      {
        product.name = productDTO.name;
        product.image = productDTO.image;
        product.CategoryId = productDTO.CategoryId;

        await _context.SaveChangesAsync();
      }
    }
    public async Task<IEnumerable<ProductDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _context.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)  
          .Skip((page - 1) * pageSize)                                      
          .Take(pageSize)                                                   
          .Select(p => new ProductDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();

      return products;
    }
    public Task<IQueryable<ProductDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      var products = _context.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
          .Select(p => new ProductDTO
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
