using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Repository
{
  public class XuathangkhoRepository : IXuathangkhoRepository
  {
    private readonly ApplicationDbContext _context;

    public XuathangkhoRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddProductAsync(XuathangDTO xuathangDTO)
    {
      var product = new Product
      {
        ProductId = xuathangDTO.ProductId,
        name = xuathangDTO.name,
        image = xuathangDTO.image,
        CategoryId = xuathangDTO.CategoryId
      };
      _context.Products.Add(product);
      await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
      return await _context.Categories.ToListAsync();
    }

    public async Task<IEnumerable<XuathangDTO>> GetProductsAsync(int categoryId)
    {
      return await _context.Products
          .Where(p => p.CategoryId == categoryId)
          .Select(p => new XuathangDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
    }

    public async Task DeleteProductAsync(int productId)
    {
      var product = await _context.Products.FindAsync(productId);
      if (product != null)
      {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<XuathangDTO> GetProductByIdAsync(int productId)
    {
      return await _context.Products
          .Where(p => p.ProductId == productId)
          .Select(p => new XuathangDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .FirstOrDefaultAsync();
    }

    public async Task UpdateProductAsync(XuathangDTO xuathangDTO)
    {
      var product = await _context.Products.FindAsync(xuathangDTO.ProductId);
      if (product != null)
      {
        product.name = xuathangDTO.name;
        product.image = xuathangDTO.image;
        product.CategoryId = xuathangDTO.CategoryId;
        await _context.SaveChangesAsync();
      }
    }

    public async Task<IEnumerable<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _context.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
          .Select(p => new XuathangDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
    }

    public async Task<IEnumerable<XuathangDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      return await _context.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(p => new XuathangDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
    }
  }
}
