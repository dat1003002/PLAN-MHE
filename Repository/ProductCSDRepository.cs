using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Repository
{
  public class ProductCSDRepository : IProductCSDRepository
  {
    private readonly ApplicationDbContext _productCSDRepository;

    public ProductCSDRepository(ApplicationDbContext productCSDRepository)
    {
      _productCSDRepository = productCSDRepository;
    }

    public async Task CreateProductAsync(ProductCSDDTO product)
    {
      var ProductCSD = new Product()
      {
        mahang = product.mahang,
        name = product.name,
        may = product.may,
        solinkthanchinh = product.solinkthanchinh,
        solinkthannoi = product.solinkthannoi,
        caosuloplot = product.caosuloplot,
        caosubemat = product.caosubemat,
        docoloplot = product.docoloplot,
        docobemat = product.docobemat,
        khuondunbemat = product.khuondunbemat,
        khuondunloplot = product.khuondunloplot,
        khotieuchuanloplot = product.khotieuchuanloplot,
        khotieuchuanbemat = product.khotieuchuanbemat,
        chieudaithanchinhloplot = product.chieudaithanchinhloplot,
        chieudaithanchinhbemat = product.chieudaithanchinhbemat,
        chieudaithannoiloplot = product.chieudaithannoiloplot,
        chieudaithannoibemat = product.chieudaithannoibemat,
        trongluongthanchinhbemat = product.chieudaithanchinhbemat,
        trongluongthanchinhloplot = product.trongluongthanchinhloplot,
        trongluongthannoibemat = product.trongluongthannoibemat,
        trongluongthannoiloplot = product.trongluongthannoiloplot,

        CategoryId = product.CategoryId
      };
      await _productCSDRepository.Products.AddAsync(ProductCSD);
      await _productCSDRepository.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int ProductId)
    {
      var product = await _productCSDRepository.Products.FindAsync(ProductId);
      if(product != null)
      {
        _productCSDRepository.Products.Remove(product);
        await _productCSDRepository.SaveChangesAsync();
      }
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productCSDRepository.Categories.ToListAsync();
    }

    public async Task<ProductCSDDTO> GetProductByIdAsync(int productId)
    {
      var productCSD = await _productCSDRepository.Products.FindAsync(productId);
      if (productCSD == null) return null;
      return new ProductCSDDTO
      {
        ProductId = productCSD.ProductId,
        name = productCSD.name,
        may = productCSD.may,
        mahang = productCSD.mahang,
        solinkthanchinh = productCSD.solinkthanchinh,
        solinkthannoi = productCSD.solinkthannoi,
        caosuloplot = productCSD.caosuloplot,
        caosubemat = productCSD.caosubemat,
        docoloplot = productCSD.docoloplot,
        docobemat = productCSD.docobemat,
        khuondunbemat = productCSD.khuondunbemat,
        khuondunloplot = productCSD.khuondunloplot,
        khotieuchuanloplot = productCSD.khotieuchuanloplot,
        khotieuchuanbemat = productCSD.khotieuchuanbemat,
        chieudaithanchinhloplot = productCSD.chieudaithanchinhloplot,
        chieudaithanchinhbemat = productCSD.chieudaithanchinhbemat,
        chieudaithannoiloplot = productCSD.chieudaithannoiloplot,
        chieudaithannoibemat = productCSD.chieudaithannoibemat,
        trongluongthanchinhbemat = productCSD.chieudaithanchinhbemat,
        trongluongthanchinhloplot = productCSD.trongluongthanchinhloplot,
        trongluongthannoibemat = productCSD.trongluongthannoibemat,
        trongluongthannoiloplot = productCSD.trongluongthannoiloplot,
        CategoryId = productCSD.CategoryId,
      };
    }

    public async Task<IQueryable<ProductCSDDTO>> GetProducts(int categoryId)
    {
      var productCSD = _productCSDRepository.Products
         .Where(p => p.CategoryId == categoryId)
         .Select(p => new ProductCSDDTO
         {
           ProductId = p.ProductId,
           name = p.name,
           mahang = p.mahang,
           may = p.may,
           solinkthanchinh = p.solinkthanchinh,
           solinkthannoi = p.solinkthannoi,
           caosuloplot = p.caosuloplot,
           caosubemat = p.caosubemat,
           docoloplot = p.docoloplot,
           docobemat = p.docobemat,
           khuondunbemat = p.khuondunbemat,
           khuondunloplot = p.khuondunloplot,
           khotieuchuanloplot = p.khotieuchuanloplot,
           khotieuchuanbemat = p.khotieuchuanbemat,
           chieudaithanchinhloplot = p.chieudaithanchinhloplot,
           chieudaithanchinhbemat = p.chieudaithanchinhbemat,
           chieudaithannoiloplot = p.chieudaithannoiloplot,
           chieudaithannoibemat = p.chieudaithannoibemat,
           trongluongthanchinhbemat = p.chieudaithanchinhbemat,
           trongluongthanchinhloplot = p.trongluongthanchinhloplot,
           trongluongthannoibemat = p.trongluongthannoibemat,
           trongluongthannoiloplot = p.trongluongthannoiloplot,

         });
      return await Task.FromResult(productCSD);
    }

    public async Task UpdateProducCSDAsync(ProductCSDDTO productCSDDTO)
    {
      var product = await _productCSDRepository.Products.FindAsync(productCSDDTO.ProductId);
      if (product != null)
      {
        product.name = productCSDDTO.name;
        product.may = productCSDDTO.may;
        product.mahang = productCSDDTO.mahang;
        product.solinkthanchinh = productCSDDTO.solinkthanchinh;
        product.solinkthannoi = productCSDDTO.solinkthannoi;
        product.caosuloplot = productCSDDTO.caosuloplot;
        product.caosubemat = productCSDDTO.caosubemat;
        product.docoloplot = productCSDDTO.docoloplot;
        product.docobemat = productCSDDTO.docobemat;
        product.khuondunloplot = productCSDDTO.khuondunloplot;
        product.khuondunbemat = productCSDDTO.khuondunbemat;
        product.khotieuchuanloplot = productCSDDTO.khotieuchuanloplot;
        product.khotieuchuanbemat = productCSDDTO.khotieuchuanbemat;
        product.chieudaithanchinhloplot = productCSDDTO.chieudaithanchinhloplot;
        product.chieudaithanchinhbemat = productCSDDTO.chieudaithanchinhbemat;
        product.chieudaithannoiloplot = productCSDDTO.chieudaithannoiloplot;
        product.chieudaithannoibemat = productCSDDTO.chieudaithannoibemat;
        product.trongluongthanchinhbemat = productCSDDTO.trongluongthanchinhbemat;
        product.trongluongthanchinhloplot = productCSDDTO.trongluongthanchinhloplot;
        product.trongluongthannoibemat = productCSDDTO.trongluongthannoibemat ;
        product.trongluongthannoiloplot = productCSDDTO.trongluongthannoiloplot;
        product.CategoryId = productCSDDTO.CategoryId;
        await _productCSDRepository.SaveChangesAsync();
      }
    }
  }
}
