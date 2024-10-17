using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList; // Nếu bạn không cần sử dụng
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AspnetCoreMvcFull.Controllers
{
  public class CaoSuCTLCotroller : Controller
  {
    private readonly IProductCSCTLService _productCSCTLService;

    public CaoSuCTLCotroller(IProductCSCTLService productCSCTLService)
    {
      _productCSCTLService = productCSCTLService;
    }
    public async Task<IActionResult> ListCaoSuCTL(int page = 1, string searchName = null)
    {
      const int categoryId = 7;
      const int pageSize = 9;

      X.PagedList.IPagedList<ProductCSCTLDTO> products;
      if (!string.IsNullOrEmpty(searchName))
      {
        products = await _productCSCTLService.SearchProductsByNameAsync(searchName, categoryId, page, pageSize);
      }
      else {
        products = await _productCSCTLService.GetProducts(categoryId, page, pageSize);
      } 
      return View("~/Views/ProductCTL/ListCaoSuCTL.cshtml",products);
    }
    public async Task<IActionResult> SearchCS(string name, int page = 1)
    {
      const int categoryId = 7;
      const int pageSize = 9;

      var products = await _productCSCTLService.SearchProductsByNameAsync(name, categoryId, page, pageSize);
      return View("~/Views/ProductCTL/ListCaoSuCTL.cshtml", products);
    }
    public async Task<IActionResult> CreateProductCSCTL()
    {
      var categories = await _productCSCTLService.GetCategories();

      var filteredCategories = categories.Where(c => c.CategoryId == 7).ToList();
      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductCTL/CreateProductCSCTL.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProductCSCTL(ProductCSCTLDTO productCSCTLDTO)
    {
      if (ModelState.IsValid)
      {
        if (productCSCTLDTO.imageFile != null)
        {
          var filePath = Path .Combine(Directory.GetCurrentDirectory(), "wwwroot/images",productCSCTLDTO.imageFile.FileName);
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await productCSCTLDTO.imageFile.CopyToAsync(stream);
          }
          productCSCTLDTO.image = productCSCTLDTO.imageFile.FileName;

        }
        await _productCSCTLService.AddProductAsync(productCSCTLDTO);
        return RedirectToAction("ListCaoSuCTL");
      }
      var categories = await _productCSCTLService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
      return View(productCSCTLDTO);
    }
    [HttpGet]
    public async Task<IActionResult> EditProductCSCTL(int id)
    {
      var product = await _productCSCTLService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      var categories = await _productCSCTLService.GetCategories();
      var filteredCategories = categories.Where(c => c.CategoryId == 7).ToList();

      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");

      return View("~/Views/ProductCTL/EditProductCSCTL.cshtml", product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductCSCTL(ProductCSCTLDTO product)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _productCSCTLService.GetCategories();
        ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
        return View("~/Views/ProductCTL/EditProductCSCTL.cshtml", product);
      }

      var existingProduct = await _productCSCTLService.GetProductByIdAsync(product.ProductId);

      if (product.imageFile != null && product.imageFile.Length > 0)
      {
        var fileName = Path.GetFileName(product.imageFile.FileName);
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

        if (!Directory.Exists(directoryPath))
        {
          Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(directoryPath, fileName);

        if (System.IO.File.Exists(filePath))
        {
          string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid() + Path.GetExtension(fileName);
          filePath = Path.Combine(directoryPath, newFileName);
          product.image = newFileName;
        }
        else
        {
          product.image = fileName;
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await product.imageFile.CopyToAsync(stream);
        }
      }
      else
      {
        product.image = existingProduct.image;
      }

      await _productCSCTLService.UpdateProductAsync(product);
      return RedirectToAction("ListCaoSuCTL", "CaoSuCTLCotroller");
    }
    public async Task<IActionResult>DeleteProductCSCTL(int ProductId)
    {
      if(ProductId <= 0)
      {
        return BadRequest("Invalid Product ID.");
      }
      await _productCSCTLService.DeleteProductAsync(ProductId);
      return RedirectToAction("ListCaoSuCTL");
    }
    public async Task<IActionResult> showProductCSCTL(int id)
    {
      var productshowCS = await _productCSCTLService.GetProductByIdAsync(id);
      if(productshowCS == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductCTL/ModalCSCTL.cshtml", productshowCS);
    }
  }
}
