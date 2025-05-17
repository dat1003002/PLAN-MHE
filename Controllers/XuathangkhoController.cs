using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace AspnetCoreMvcFull.Controllers
{
  public class XuathangkhoController : Controller
  {
    private readonly IXuathangSevice _xuathangService;
    private const int CategoryId = 11; // Mặc định CategoryId = 10
    private const int PageSize = 9; // Số sản phẩm mỗi trang

    public XuathangkhoController(IXuathangSevice xuathangService)
    {
      _xuathangService = xuathangService;
    }

    // GET: /Xuathangkho/Listxuathangkho
    public async Task<IActionResult> Listxuathangkho(int page = 1)
    {
      var products = await _xuathangService.GetProductsAsync(CategoryId, page, PageSize);
      TempData["SearchTerm"] = null;
      return View("~/Views/ProductKho/Listxuathangkho.cshtml", products);
    }

    // GET: /Xuathangkho/SearchProductXH
    public async Task<IActionResult> SearchProductXH(string name, int page = 1)
    {
      if (string.IsNullOrEmpty(name))
      {
        return RedirectToAction(nameof(Listxuathangkho));
      }

      var productsXH = await _xuathangService.SearchProductsByNameAsync(name, CategoryId, page, PageSize);
      TempData["SearchTerm"] = name; // Lưu từ khóa tìm kiếm vào TempData
      TempData.Keep("SearchTerm"); // Giữ TempData qua các yêu cầu tiếp theo
      return View("~/Views/ProductKho/Listxuathangkho.cshtml", productsXH);
    }

    // GET: /Xuathangkho/CreatekhoXH
    public async Task<IActionResult> CreatekhoXH()
    {
      var categories = await _xuathangService.GetCategoriesAsync();
      var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductKho/CreatekhoXH.cshtml");
    }

    // POST: /Xuathangkho/CreatekhoXH
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatekhoXH(XuathangDTO product)
    {
      if (ModelState.IsValid)
      {
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
        await _xuathangService.AddProductAsync(product);
        return RedirectToAction(nameof(Listxuathangkho));
      }

      var categories = await _xuathangService.GetCategoriesAsync();
      var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductKho/CreatekhoXH.cshtml", product);
    }

    // GET: /Xuathangkho/Edit
    public async Task<IActionResult> EditKhoXH(int id)
    {
      var product = await _xuathangService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      var categories = await _xuathangService.GetCategoriesAsync();
      var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductKho/EditKhoXH.cshtml", product);
    }

    // POST: /Xuathangkho/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditKhoXH(XuathangDTO product)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _xuathangService.GetCategoriesAsync();
        var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
        ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
        return View("~/Views/ProductKho/EditKhoXH.cshtml", product);
      }

      var existingProduct = await _xuathangService.GetProductByIdAsync(product.ProductId);
      if (existingProduct == null)
      {
        return NotFound();
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

      await _xuathangService.UpdateProductAsync(product);
      return RedirectToAction(nameof(Listxuathangkho));
    }

    // POST: /Xuathangkho/Delete
    [HttpPost]
    public async Task<IActionResult> DeleteKhoXH(int productId)
    {
      if (productId <= 0)
      {
        return BadRequest("Invalid Product ID.");
      }
      await _xuathangService.DeleteProductAsync(productId);
      return Json(new { success = true, message = "Sản phẩm đã được xóa thành công!" });
    }

    // GET: /Xuathangkho/Details
    public async Task<IActionResult> ShowProductKhoXH(int id)
    {
      var product = await _xuathangService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductKho/ShowProductKhoXH.cshtml", product);
    }
  }
}
