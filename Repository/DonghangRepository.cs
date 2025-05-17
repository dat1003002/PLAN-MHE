using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  public class DongHangRepository : IDonghangRepository
  {
    private readonly ApplicationDbContext _context;

    public DongHangRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddProductAsync(DonghangkhoDTO donghangkhoDTO)
    {
      var product = new Product
      {
        ProductId = donghangkhoDTO.ProductId,
        name = donghangkhoDTO.name,
        image = donghangkhoDTO.image,
        CategoryId = donghangkhoDTO.CategoryId
      };
      _context.Products.Add(product);
      await _context.SaveChangesAsync();
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

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _context.Categories.ToListAsync();
    }

    public async Task<DonghangkhoDTO> GetProductByIdAsync(int productId)
    {
      return await _context.Products
          .Where(p => p.ProductId == productId)
          .Select(p => new DonghangkhoDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          }).FirstOrDefaultAsync();
    }

    public Task<IQueryable<DonghangkhoDTO>> GetProducts(int categoryId)
    {
      var query = _context.Products
          .Where(p => p.CategoryId == categoryId)
          .Select(p => new DonghangkhoDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          });
      return Task.FromResult(query);
    }

    public Task<IQueryable<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      var products = _context.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
          .Select(p => new DonghangkhoDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          });
      return Task.FromResult(products);
    }

    public async Task<IEnumerable<DonghangkhoDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      return await _context.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(p => new DonghangkhoDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
    }

    public async Task UpdateProductAsync(DonghangkhoDTO donghangkhoDTO)
    {
      var product = await _context.Products.FindAsync(donghangkhoDTO.ProductId);
      if (product != null)
      {
        product.name = donghangkhoDTO.name;
        product.image = donghangkhoDTO.image;
        product.CategoryId = donghangkhoDTO.CategoryId;
        await _context.SaveChangesAsync();
      }
    }
  }
}
