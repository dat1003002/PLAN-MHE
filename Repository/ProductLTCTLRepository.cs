using AspnetCoreMvcFull.Data;
using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreMvcFull.Repository
{
  public class ProductLTCTLRepository : IProductLTCTLRepository
  {
    private readonly ApplicationDbContext _productLTCTLRepository;

    public ProductLTCTLRepository(ApplicationDbContext productLTCTLRepository)
    {
      _productLTCTLRepository = productLTCTLRepository;
    }

    public async Task CreateProductAsync(LoiThepCTLDTO product)
    {
      var ProductLT = new Product()
      {
        mahangctl = product.mahangctl,
        name = product.name,
        chieudailoithep = product.chieudailoithep,
        khoangcach2daumoinoiloithep = product.khoangcach2daumoinoiloithep,
        khocaosubo = product.khocaosubo,
        khocaosuketdinh3t = product.khocaosuketdinh3t,
        kholoithep = product.kholoithep,
        kichthuoccuacaosudanmoinoi = product.kichthuoccuacaosudanmoinoi,
        solink = product.solink,
        sosoiloithep = product.sosoiloithep,
        tocdoquan = product.tocdoquan,
        trongluongloithepspinning = product.trongluongloithepspinning,
        dodaycaosubo = product.dodaycaosubo,
        dodaycaosuketdinh3t = product.dodaycaosuketdinh3t,

        CategoryId = product.CategoryId,

      };

      await _productLTCTLRepository.Products.AddAsync(ProductLT);
      await _productLTCTLRepository.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int productId)
    {
      var product = await _productLTCTLRepository.Products.FindAsync(productId);
      if (product != null)
      {
        _productLTCTLRepository.Products.Remove(product);
        await _productLTCTLRepository.SaveChangesAsync();
      }
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
      return await _productLTCTLRepository.Categories.ToListAsync();
    }

    public async Task<LoiThepCTLDTO> GetProductByIdAsync(int productId)
    {
      var productLT = await _productLTCTLRepository.Products.FindAsync(productId);
      if (productLT == null) return null;

      return new LoiThepCTLDTO
      {
        ProductId = productLT.ProductId,
        mahangctl = productLT.mahangctl,
        name = productLT.name,
        chieudailoithep = productLT.chieudailoithep,
        khoangcach2daumoinoiloithep = productLT.khoangcach2daumoinoiloithep,
        khocaosubo = productLT.khocaosubo,
        khocaosuketdinh3t = productLT.khocaosuketdinh3t,
        kholoithep = productLT.kholoithep,
        kichthuoccuacaosudanmoinoi = productLT.kichthuoccuacaosudanmoinoi,
        solink = productLT.solink,
        sosoiloithep = productLT.sosoiloithep,
        tocdoquan = productLT.tocdoquan,
        trongluongloithepspinning = productLT.trongluongloithepspinning,
        dodaycaosubo = productLT.dodaycaosubo,
        dodaycaosuketdinh3t = productLT.dodaycaosuketdinh3t,
        CategoryId = productLT.CategoryId,
      };
    }

    public async Task<IQueryable<LoiThepCTLDTO>> GetProducts(int categoryId)
    {
      var productLT = _productLTCTLRepository.Products
        .Where(p => p.CategoryId == categoryId)
        .Select(p => new LoiThepCTLDTO
        {
          ProductId = p.ProductId,
          mahangctl = p.mahangctl,
          name = p.name,
          chieudailoithep = p.chieudailoithep,
          khoangcach2daumoinoiloithep = p.khoangcach2daumoinoiloithep,
          khocaosubo = p.khocaosubo,
          khocaosuketdinh3t = p.khocaosuketdinh3t,
          kholoithep = p.kholoithep,
          kichthuoccuacaosudanmoinoi = p.kichthuoccuacaosudanmoinoi,
          solink = p.solink,
          sosoiloithep = p.sosoiloithep,
          tocdoquan = p.tocdoquan,
          trongluongloithepspinning = p.trongluongloithepspinning,
          dodaycaosubo = p.dodaycaosubo,
          dodaycaosuketdinh3t = p.dodaycaosuketdinh3t,
          CategoryId = p.CategoryId,

        });
      return await Task.FromResult(productLT);
    }

    public async Task UpdateProductAsync(LoiThepCTLDTO loiThepCTLDTO)
    {
      var productLT = await _productLTCTLRepository.Products.FindAsync(loiThepCTLDTO.ProductId);
      if (productLT != null)
      {

        productLT.mahangctl = loiThepCTLDTO.mahangctl;
        productLT.name = loiThepCTLDTO.name;
        productLT.chieudailoithep = loiThepCTLDTO.chieudailoithep;
        productLT.khoangcach2daumoinoiloithep = loiThepCTLDTO.khoangcach2daumoinoiloithep;
        productLT.khocaosubo = loiThepCTLDTO.khocaosubo;
        productLT.khocaosuketdinh3t = loiThepCTLDTO.khocaosuketdinh3t;
        productLT.kholoithep = loiThepCTLDTO.kholoithep;
        productLT.kichthuoccuacaosudanmoinoi = loiThepCTLDTO.kichthuoccuacaosudanmoinoi;
        productLT.solink = loiThepCTLDTO.solink;
        productLT.sosoiloithep = loiThepCTLDTO.sosoiloithep;
        productLT.tocdoquan = loiThepCTLDTO.tocdoquan;
        productLT.trongluongloithepspinning = loiThepCTLDTO.trongluongloithepspinning;
        productLT.dodaycaosubo = loiThepCTLDTO.dodaycaosubo;
        productLT.dodaycaosuketdinh3t = loiThepCTLDTO.dodaycaosuketdinh3t;
        productLT.CategoryId = loiThepCTLDTO.CategoryId;
        productLT.UpdatedAt = DateTime.Now;

        await _productLTCTLRepository.SaveChangesAsync();
      }
    }
  }
}
