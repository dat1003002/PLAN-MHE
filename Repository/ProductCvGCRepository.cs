using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task DeleteProductAsync(int ProductId)
    {
      var product = await _DBGCcontext.Products.FindAsync(ProductId);
      if (product != null)
      {
        _DBGCcontext.Products.Remove(product);
        await _DBGCcontext.SaveChangesAsync();
      }
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
      return await _DBGCcontext.Categories.ToListAsync();
    }

    public async Task<ProductGCMHEDTO> GetProductByIdAsync(int id)
    {
      return await _DBGCcontext.Products
          .Where(p => p.ProductId == id)
          .Select(p => new ProductGCMHEDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
            // thêm các thuộc tính khác nếu cần
          }).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProductGCMHEDTO>> GetProductsByCategoryAsync(int categoryId)
    {
      return await _DBGCcontext.Products
          .Where(p => p.CategoryId == categoryId)
          .Select(p => new ProductGCMHEDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
    }
    public async Task<IEnumerable<ProductGCMHEDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _DBGCcontext.Products
          .Where(p => p.CategoryId == categoryId && p.name.Contains(name))
          .Select(p => new ProductGCMHEDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
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
  }
}
