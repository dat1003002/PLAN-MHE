using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AspnetCoreMvcFull.Controllers
{
  public class ProductCvGCCtroller : Controller
  {
    private readonly IProductCvGCService _productService;

    public ProductCvGCCtroller(IProductCvGCService productService)
    {
      _productService = productService;
    }

    public async Task<IActionResult> ListCvGCMHE(int page = 1, string searchName = null)
    {
      const int categoryId = 4;
      const int pageSize = 10;

      X.PagedList.IPagedList<ProductGCMHEDTO> products;
      if (!string.IsNullOrEmpty(searchName))
      {
        products = await _productService.SearchProductsByNameAsync(searchName, categoryId, page, pageSize);
      }
      else
      {
        products = await _productService.GetProducts(categoryId, page, pageSize);
      }

      return View("~/Views/ProductMhe/ListCvGCMHE.cshtml", products);
    }
    [HttpPost]
    public async Task<IActionResult> Search(string name, int page = 1)
    {
      const int categoryId = 4;
      const int pageSize = 10;

      var products = await _productService.SearchProductsByNameAsync(name, categoryId, page, pageSize);

      return View("~/Views/ProductMhe/ListCvGCMHE.cshtml", products);
    }
    public async Task<IActionResult> CreateProductGC()
    {
      var categories = await _productService.GetCategories();
      var filterCreatecategori = categories.Where(c => c.CategoryId == 4).ToList();
      ViewBag.CategoryList = new SelectList(filterCreatecategori, "CategoryId", "CategoryName");
      return View("~/Views/ProductMhe/CreateProductGC.cshtml");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProductGC(ProductGCMHEDTO product)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _productService.GetCategories();
        ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
        return View("~/Views/ProductMhe/CreateProductGC.cshtml", product);
      }
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
          // Thay đổi tên file nếu đã tồn tại
          string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid() + Path.GetExtension(fileName);
          filePath = Path.Combine(directoryPath, newFileName);
          product.image = newFileName;
        }
        else
        {
          product.image = fileName;
        }

        // Lưu file ảnh
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await product.imageFile.CopyToAsync(stream);
        }
      }
      await _productService.AddProductAsync(product);
      return RedirectToAction("ListCvGCMHE", "ProductCvGCCtroller");
    }
    public async Task<IActionResult> EditProductGCMHE(int id)
    {
      var product = await _productService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      var categories = await _productService.GetCategories();
      var filterCreatecategori = categories.Where(c => c.CategoryId == 4).ToList();
      ViewBag.CategoryList = new SelectList(filterCreatecategori, "CategoryId", "CategoryName", product.CategoryId);
      return View("~/Views/ProductMhe/EditProductGCMHE.cshtml", product);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductGCMHE(ProductGCMHEDTO product)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _productService.GetCategories();
        ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
        return View("~/Views/ProductMhe/EditProductGCMHE.cshtml", product);
      }

      var existingProduct = await _productService.GetProductByIdAsync(product.ProductId);

      // Xử lý việc lưu ảnh nếu có thay đổi
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
        // Nếu không có hình ảnh mới, giữ lại hình ảnh cũ
        product.image = existingProduct.image;
      }

      // Cập nhật sản phẩm
      await _productService.UpdateProductAsync(product);
      return RedirectToAction("ListCvGCMHE", "ProductCvGCCtroller");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteProductCVGC(int ProductId)
    {
      if (ProductId <= 0)
      {
        return BadRequest("Invalid Product ID.");
      }

      await _productService.DeleteProductAsync(ProductId);
      return RedirectToAction("ListCvGCMHE");
    }
    public async Task<IActionResult> ShowProductGcById(int id)
    {
      var product = await _productService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductMhe/ProductGCMHEModal.cshtml", product);
    }
  }
}
