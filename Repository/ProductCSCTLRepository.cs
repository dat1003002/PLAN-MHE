using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Repository
{
  public class ProductCSCTLRepository : IProductCSCTLRepository
  {
    private readonly ApplicationDbContext _productCSCTLRepository;
    public ProductCSCTLRepository(ApplicationDbContext productCSCTLRepository)
    {
      _productCSCTLRepository = productCSCTLRepository;
    }
    public async Task AddProductAsync(ProductCSCTLDTO productCSCTLDTO)
    {
      var addProductCS = new Product
      {
        name = productCSCTLDTO.name,
        image = productCSCTLDTO.image,
        CategoryId = productCSCTLDTO.CategoryId,
      };
      await _productCSCTLRepository.Products.AddAsync(addProductCS);
      await _productCSCTLRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productCSCTLRepository.Categories.ToListAsync();
    }
    public async Task<IQueryable<ProductCSCTLDTO>> GetProducts(int categoryId)
    {
      var products = _productCSCTLRepository.Products
        .Where(p => p.CategoryId == categoryId)
        .Select(p => new ProductCSCTLDTO
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
     var product = await _productCSCTLRepository.Products.FindAsync (ProductId);
      if(product != null)
      {
        _productCSCTLRepository.Products.Remove(product);
        await _productCSCTLRepository.SaveChangesAsync();
      }
    }
    public async Task<ProductCSCTLDTO> GetProductByIdAsync(int productId)
    {
     return await _productCSCTLRepository.Products
        .Where (p => p.ProductId == productId)
        .Select(p => new ProductCSCTLDTO
        {
          ProductId = p.ProductId,
          name = p.name,
          image = p.image,
          CategoryId = p.CategoryId,
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateProductAsync(ProductCSCTLDTO product)
    {
      var editProduct = await _productCSCTLRepository.Products.FindAsync(product.ProductId);
      if (editProduct != null)
      {
        editProduct.name = product.name;
        editProduct.image = product.image;
        editProduct.CategoryId = product.CategoryId;
        await _productCSCTLRepository.SaveChangesAsync();
      }
    }
    public async Task<IEnumerable<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var products = await _productCSCTLRepository.Products
        .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProductCSCTLDTO
        {
          ProductId = p.ProductId,
          name = p.name,
          image = p.image,
          CategoryId = p.CategoryId
        }).ToListAsync();
   return products;
      
    }
    public Task<IQueryable<ProductCSCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      var products = _productCSCTLRepository.Products
         .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
         .Select(p => new ProductCSCTLDTO
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
