using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Controllers
{
  public class ProductMheController : Controller
  {
    private readonly IProductLTService _productService;

    public ProductMheController(IProductLTService productService)
    {
      _productService = productService;
    }
    public async Task<IActionResult> CreateProductLT()
    {
      var categories = await _productService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
      return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProductLT(Product product)
    {
      try
      {
        await _productService.AddProductAsync(product);
        return RedirectToAction(nameof(ListLoiThep));
      }
      catch (Exception ex)
      {
        // Log the exception
        Console.WriteLine(ex.Message);
        ModelState.AddModelError("", "Có lỗi xảy ra khi lưu sản phẩm.");
      }

      var categories = await _productService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
      return View(product);
    }
    public async Task<IActionResult> DeleteProductLT(int id)
    {
      try
      {
        await _productService.DeleteProductAsync(id);
        return Json(new { success = true });
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm." });
      }
    }
    public async Task<IActionResult> EditProductLT(int id)
    {
      var product = await _productService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      var categories = await _productService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);

      return View(product);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductLT(Product product)
    {
      try
      {
        await _productService.UpdateProductAsync(product);
        return RedirectToAction(nameof(ListLoiThep));
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật sản phẩm.");
      }

      // Tải lại các danh mục và trả về view với chi tiết sản phẩm hiện tại
      var categories = await _productService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
      return View(product);
    }
   
    public async Task<IActionResult> ListLoiThep()
    {
      int categoryId = 1;
      var products = await _productService.GetProductsByCategoryIdAsync(categoryId);
      return View(products);
    }

    public async Task<IActionResult> ShowProductLTById(int id)
    {
      var product = await _productService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      ViewBag.ProductInfo = product;

      return PartialView("_ProductModal");
    }

  }
}
