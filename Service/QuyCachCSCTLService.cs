using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Service
{
  public class QuyCachCSCTLService : IQuyCachCSCTLService
  {
    private readonly IQuyCachCSCTLRepository _repository;

    public QuyCachCSCTLService(IQuyCachCSCTLRepository repository)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task CreateProductAsync(QuyCachCaoSuCTLDTO product)
    {
      if (product == null)
        throw new ArgumentNullException(nameof(product));

      await _repository.CreateProductAsync(product);
    }

    public async Task DeleteProductAsync(int productId)
    {
      if (productId <= 0)
        throw new ArgumentException("Invalid product ID.", nameof(productId));

      await _repository.DeleteProductAsync(productId);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _repository.GetCategories();
    }

    public async Task<QuyCachCaoSuCTLDTO> GetProductByIdAsync(int productId)
    {
      if (productId <= 0)
        throw new ArgumentException("Invalid product ID.", nameof(productId));

      var product = await _repository.GetProductByIdAsync(productId);
      return product ?? throw new InvalidOperationException($"Product with ID {productId} not found.");
    }

    public async Task<IEnumerable<QuyCachCaoSuCTLDTO>> GetProducts(int categoryId)
    {
      if (categoryId <= 0)
        throw new ArgumentException("Invalid category ID.", nameof(categoryId));

      var products = await _repository.GetProducts(categoryId);
      return await products.ToListAsync();
    }

    public async Task UpdateProductAsync(QuyCachCaoSuCTLDTO quyCachCaoSuCTLDTO)
    {
      if (quyCachCaoSuCTLDTO == null)
        throw new ArgumentNullException(nameof(quyCachCaoSuCTLDTO));

      if (quyCachCaoSuCTLDTO.ProductId <= 0)
        throw new ArgumentException("Invalid product ID.", nameof(quyCachCaoSuCTLDTO.ProductId));

      await _repository.UpdateProductAsync(quyCachCaoSuCTLDTO);
    }
  }
}
