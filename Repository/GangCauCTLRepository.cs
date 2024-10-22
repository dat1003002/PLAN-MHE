using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Repository
{
  public class GangCauCTLRepository : IGangCauCTLRepository
  {
    private readonly ApplicationDbContext _gangCauCTLRepository;
    public GangCauCTLRepository(ApplicationDbContext gangCauCTLRepository)
    {
      _gangCauCTLRepository = gangCauCTLRepository;
    }

    public async Task AddProductAsync(ProductGCCTLDTO productGCCTLDTO)
    {
      var addProductGCCTL = new Product
      {
        name = productGCCTLDTO.name,
        image = productGCCTLDTO.image,
        CategoryId = productGCCTLDTO.CategoryId,
      };
      await _gangCauCTLRepository.Products.AddAsync(addProductGCCTL);
      await _gangCauCTLRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _gangCauCTLRepository.Categories.ToListAsync();
    }
    public async Task<IQueryable<ProductGCCTLDTO>> GetProducts(int categoryId)
    {
      var products = _gangCauCTLRepository.Products
       .Where(p => p.CategoryId == categoryId)
       .Select(p => new ProductGCCTLDTO
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
      var productDLGCCTL = await _gangCauCTLRepository.Products.FindAsync(ProductId);
      if (productDLGCCTL != null)
      {
        _gangCauCTLRepository.Products.Remove(productDLGCCTL);
        await _gangCauCTLRepository.SaveChangesAsync();
      }
    }
    public async Task<ProductGCCTLDTO> GetProductByIdAsync(int productId)
    {
      return await _gangCauCTLRepository.Products
        .Where(p => p.ProductId == productId)
        .Select(p => new ProductGCCTLDTO
        {
          ProductId = p.ProductId,
          name = p.name,
          image = p.image,
          CategoryId = p.CategoryId,
        }).FirstOrDefaultAsync();
    }
    public async Task UpdateProductAsync(ProductGCCTLDTO product)
    {
      var editProductGC = await _gangCauCTLRepository.Products.FindAsync(product.ProductId);
      if (editProductGC != null)
      {
        editProductGC.name = product.name;
        editProductGC.image = product.image;
        editProductGC.CategoryId = product.CategoryId;
        await _gangCauCTLRepository.SaveChangesAsync();
      }
    }
    public async Task<IEnumerable<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId, int page, int pageSize)
    {
      var productGC = await _gangCauCTLRepository.Products
        .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProductGCCTLDTO
        {
          ProductId = p.ProductId,
          name = p.name,
          image = p.image,
          CategoryId = p.CategoryId
        }).ToListAsync();
      return productGC;
    }
    public Task<IQueryable<ProductGCCTLDTO>> SearchProductsByNameAsync(string name, int categoryId)
    {
      var products = _gangCauCTLRepository.Products
         .Where(p => p.name.Contains(name) && p.CategoryId == categoryId)
         .Select(p => new ProductGCCTLDTO
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
