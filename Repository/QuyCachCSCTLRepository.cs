using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Repository
{
  public class QuyCachCSCTLRepository : IQuyCachCSCTLRepository
  {
    private readonly ApplicationDbContext _context;

    public QuyCachCSCTLRepository(ApplicationDbContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Create a new product with all fields from QuyCachCaoSuCTLDTO
    public async Task CreateProductAsync(QuyCachCaoSuCTLDTO product)
    {
      if (product == null)
        throw new ArgumentNullException(nameof(product));

      var productEntity = new Product
      {
        mahangctl = product.mahangctl,
        name = product.name,
        loaicaosu = product.loaicaosu,
        loaicaosu1 = product.loaicaosu1,
        loaicaosu2 = product.loaicaosu2,
        loaikhuondun = product.loaikhuondun,
        loaikhuondun1 = product.loaikhuondun1,
        loaikhuondun2 = product.loaikhuondun2,
        tocdomotor = product.tocdomotor,
        tocdomotor1 = product.tocdomotor1,
        tocdomotor2 = product.tocdomotor2,
        chieudai = product.chieudai,
        chieudai1 = product.chieudai1,
        chieudai2 = product.chieudai2,
        kho = product.kho,
        kho1 = product.kho1,
        kho2 = product.kho2,
        doday = product.doday,
        doday1 = product.doday1,
        doday2 = product.doday2,
        trongluong = product.trongluong,
        trongluong1 = product.trongluong1,
        trongluong2 = product.trongluong2,
        CreatedAt = DateTime.Now,
        CategoryId = product.CategoryId
      };

      await _context.Products.AddAsync(productEntity);
      await _context.SaveChangesAsync();
    }

    // Delete a product by ID
    public async Task DeleteProductAsync(int productId)
    {
      var product = await _context.Products.FindAsync(productId);
      if (product != null)
      {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
      }
    }

    // Get all categories
    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _context.Categories.ToListAsync();
    }

    // Get a product by ID with all fields
    public async Task<QuyCachCaoSuCTLDTO> GetProductByIdAsync(int productId)
    {
      var product = await _context.Products.FindAsync(productId);
      if (product == null)
        return null;

      return new QuyCachCaoSuCTLDTO
      {
        ProductId = product.ProductId,
        mahangctl = product.mahangctl,
        name = product.name,
        loaicaosu = product.loaicaosu,
        loaicaosu1 = product.loaicaosu1,
        loaicaosu2 = product.loaicaosu2,
        loaikhuondun = product.loaikhuondun,
        loaikhuondun1 = product.loaikhuondun1,
        loaikhuondun2 = product.loaikhuondun2,
        tocdomotor = product.tocdomotor,
        tocdomotor1 = product.tocdomotor1,
        tocdomotor2 = product.tocdomotor2,
        chieudai = product.chieudai,
        chieudai1 = product.chieudai1,
        chieudai2 = product.chieudai2,
        kho = product.kho,
        kho1 = product.kho1,
        kho2 = product.kho2,
        doday = product.doday,
        doday1 = product.doday1,
        doday2 = product.doday2,
        trongluong = product.trongluong,
        trongluong1 = product.trongluong1,
        trongluong2 = product.trongluong2,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        CategoryId = product.CategoryId
      };
    }

    // Get products by category ID with all fields
    public async Task<IQueryable<QuyCachCaoSuCTLDTO>> GetProducts(int categoryId)
    {
      return await Task.FromResult(_context.Products
        .Where(p => p.CategoryId == categoryId)
        .Select(p => new QuyCachCaoSuCTLDTO
        {
          ProductId = p.ProductId,
          mahangctl = p.mahangctl,
          name = p.name,
          loaicaosu = p.loaicaosu,
          loaicaosu1 = p.loaicaosu1,
          loaicaosu2 = p.loaicaosu2,
          loaikhuondun = p.loaikhuondun,
          loaikhuondun1 = p.loaikhuondun1,
          loaikhuondun2 = p.loaikhuondun2,
          tocdomotor = p.tocdomotor,
          tocdomotor1 = p.tocdomotor1,
          tocdomotor2 = p.tocdomotor2,
          chieudai = p.chieudai,
          chieudai1 = p.chieudai1,
          chieudai2 = p.chieudai2,
          kho = p.kho,
          kho1 = p.kho1,
          kho2 = p.kho2,
          doday = p.doday,
          doday1 = p.doday1,
          doday2 = p.doday2,
          trongluong = p.trongluong,
          trongluong1 = p.trongluong1,
          trongluong2 = p.trongluong2,
          CreatedAt = p.CreatedAt,
          UpdatedAt = p.UpdatedAt,
          CategoryId = p.CategoryId
        }));
    }

    // Update a product with all fields
    public async Task UpdateProductAsync(QuyCachCaoSuCTLDTO quyCachCaoSuCTLDTO)
    {
      if (quyCachCaoSuCTLDTO == null)
        throw new ArgumentNullException(nameof(quyCachCaoSuCTLDTO));

      var product = await _context.Products.FindAsync(quyCachCaoSuCTLDTO.ProductId);
      if (product == null)
        throw new InvalidOperationException("Product not found.");

      product.mahangctl = quyCachCaoSuCTLDTO.mahangctl;
      product.name = quyCachCaoSuCTLDTO.name;
      product.loaicaosu = quyCachCaoSuCTLDTO.loaicaosu;
      product.loaicaosu1 = quyCachCaoSuCTLDTO.loaicaosu1;
      product.loaicaosu2 = quyCachCaoSuCTLDTO.loaicaosu2;
      product.loaikhuondun = quyCachCaoSuCTLDTO.loaikhuondun;
      product.loaikhuondun1 = quyCachCaoSuCTLDTO.loaikhuondun1;
      product.loaikhuondun2 = quyCachCaoSuCTLDTO.loaikhuondun2;
      product.tocdomotor = quyCachCaoSuCTLDTO.tocdomotor;
      product.tocdomotor1 = quyCachCaoSuCTLDTO.tocdomotor1;
      product.tocdomotor2 = quyCachCaoSuCTLDTO.tocdomotor2;
      product.chieudai = quyCachCaoSuCTLDTO.chieudai;
      product.chieudai1 = quyCachCaoSuCTLDTO.chieudai1;
      product.chieudai2 = quyCachCaoSuCTLDTO.chieudai2;
      product.kho = quyCachCaoSuCTLDTO.kho;
      product.kho1 = quyCachCaoSuCTLDTO.kho1;
      product.kho2 = quyCachCaoSuCTLDTO.kho2;
      product.doday = quyCachCaoSuCTLDTO.doday;
      product.doday1 = quyCachCaoSuCTLDTO.doday1;
      product.doday2 = quyCachCaoSuCTLDTO.doday2;
      product.trongluong = quyCachCaoSuCTLDTO.trongluong;
      product.trongluong1 = quyCachCaoSuCTLDTO.trongluong1;
      product.trongluong2 = quyCachCaoSuCTLDTO.trongluong2;
      product.CategoryId = quyCachCaoSuCTLDTO.CategoryId;
      product.UpdatedAt = DateTime.Now;

      _context.Products.Update(product);
      await _context.SaveChangesAsync();
    }
  }
}
