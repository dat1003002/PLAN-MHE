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
  public class DonghangController : Controller
  {
    private readonly IDongHangService _dongHangService;
    private const int CategoryId = 10; // Mặc định CategoryId = 10
    private const int PageSize = 9; // Số sản phẩm mỗi trang

    public DonghangController(IDongHangService dongHangService)
    {
      _dongHangService = dongHangService;
    }

    // GET: /Donghang/Listdonghang
    public async Task<IActionResult> Listdonghang(int page = 1)
    {
      var products = await _dongHangService.GetProducts(CategoryId, page, PageSize);
      TempData["SearchTerm"] = null; // Xóa từ khóa tìm kiếm khi xem danh sách mặc định
      return View("~/Views/ProductKho/Listdonghang.cshtml", products);
    }

    // GET: /Donghang/SearchPoductDH
    public async Task<IActionResult> SearchPoductDH(string name, int page = 1)
    {
      if (string.IsNullOrEmpty(name))
      {
        return RedirectToAction(nameof(Listdonghang));
      }

      var productsDH = await _dongHangService.SearchProductsByNameAsync(name, CategoryId, page, PageSize);
      TempData["SearchTerm"] = name; // Lưu từ khóa tìm kiếm vào TempData
      TempData.Keep("SearchTerm"); // Giữ TempData qua các yêu cầu tiếp theo
      return View("~/Views/ProductKho/Listdonghang.cshtml", productsDH);
    }

    // GET: /Donghang/CreatekhoDH
    public async Task<IActionResult> CreatekhoDH()
    {
      var categories = await _dongHangService.GetCategories();
      var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductKho/CreatekhoDH.cshtml");
    }

    // POST: /Donghang/CreatekhoDH
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatekhoDH(DonghangkhoDTO product)
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
        await _dongHangService.AddProductAsync(product);
        return RedirectToAction(nameof(Listdonghang));
      }

      var categories = await _dongHangService.GetCategories();
      var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductKho/CreatekhoDH.cshtml", product);
    }

    // GET: /Donghang/Edit
    public async Task<IActionResult> EditKhoDH(int id)
    {
      var product = await _dongHangService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      var categories = await _dongHangService.GetCategories();
      var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
      ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductKho/EditKhoDH.cshtml", product);
    }

    // POST: /Donghang/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditKhoDH(DonghangkhoDTO product)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _dongHangService.GetCategories();
        var filteredCategories = categories.Where(c => c.CategoryId == CategoryId).ToList();
        ViewBag.CategoryList = new SelectList(filteredCategories, "CategoryId", "CategoryName");
        return View("~/Views/ProductKho/EditKhoDH.cshtml", product);
      }

      var existingProduct = await _dongHangService.GetProductByIdAsync(product.ProductId);
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

      await _dongHangService.UpdateProductAsync(product);
      return RedirectToAction(nameof(Listdonghang));
    }

    // POST: /Donghang/Delete
    [HttpPost]
    public async Task<IActionResult> DeleteKhoDH(int ProductId)
    {
      if (ProductId <= 0)
      {
        return BadRequest("Invalid Product ID.");
      }
      await _dongHangService.DeleteProductAsync(ProductId);
      return Json(new { success = true, message = "Sản phẩm đã được xóa thành công!" });
    }

    // GET: /Donghang/Details
    public async Task<IActionResult> showProductKhoDH(int id)
    {
      var product = await _dongHangService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductKho/showProductKhoDH.cshtml", product);
    }
  }
}
