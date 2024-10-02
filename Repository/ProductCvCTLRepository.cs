using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Repository
{
  public class ProductCvCTLRepository : IProductCvCTLRepository
  {
    private readonly ApplicationDbContext _dbContext;

    public ProductCvCTLRepository(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task CreateProductAsync(ProductCTLDTO product)
    {
      var newProduct = new Product // Giả sử bạn có lớp Product trong models
      {
        name = product.name,
        image = product.image,
        CategoryId = product.CategoryId
      };

      await _dbContext.Products.AddAsync(newProduct);
      await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      var product = await _dbContext.Products.FindAsync(ProductId);
      if (product != null)
      {
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
      }
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
      return await _dbContext.Categories.ToListAsync();
    }

    public async Task<ProductCTLDTO> GetProductByIdAsync(int productId)
    {
      var product = await _dbContext.Products.FindAsync(productId);
      if (product == null) return null;

      return new ProductCTLDTO
      {
        ProductId = product.ProductId,
        name = product.name,
        image = product.image,
        CategoryId = product.CategoryId
      };
    }

    public async Task<List<ProductCTLDTO>> GetProductsByCategoryIdAsync(int categoryId)
    {
      return await _dbContext.Products
          .Where(p => p.CategoryId == categoryId)
          .Select(p => new ProductCTLDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
    }

    public async Task<List<ProductCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      return await _dbContext.Products
          .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
          .Select(p => new ProductCTLDTO
          {
            ProductId = p.ProductId,
            name = p.name,
            image = p.image,
            CategoryId = p.CategoryId
          })
          .ToListAsync();
    }

    //public async Task<List<ProductCTLDTO>> SearchProductsByNameAsync(string name)
    //{
    //  return await _dbContext.Products
    //        .Where(p => p.name.Contains(name))
    //        .Select(p => new ProductCTLDTO
    //        {
    //          ProductId = p.ProductId,
    //          name = p.name,
    //          image = p.image,
    //          CategoryId = p.CategoryId
    //        })
    //        .ToListAsync();
    //}

    public async Task UpdateProductAsync(ProductCTLDTO product)
    {
      var existingProduct = await _dbContext.Products.FindAsync(product.ProductId);
      if (existingProduct != null)
      {
        existingProduct.name = product.name;
        existingProduct.image = product.image;
        existingProduct.CategoryId = product.CategoryId;

        _dbContext.Products.Update(existingProduct);
        await _dbContext.SaveChangesAsync();
      }
    }
  }
}
