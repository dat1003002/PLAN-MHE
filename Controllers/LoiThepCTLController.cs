using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AspnetCoreMvcFull.Controllers
{
  public class LoiThepCTLController : Controller
  {
    private readonly IProductLTCTLService _productLTCTLService;

    public LoiThepCTLController(IProductLTCTLService productLTCTLService)
    {
      _productLTCTLService = productLTCTLService;
    }
    public async Task<IActionResult> ListLoiThepCTL()
    {
      int categoryId = 12;
      var products = await _productLTCTLService.GetProducts(categoryId);
      return View("~/Views/ProductCTL/ListLoiThepCTL.cshtml", products);
    }

    public async Task<IActionResult> CreateProductLT()
    {
      var categories = await _productLTCTLService.GetCategories();
      var filtercategories = categories.Where(c => c.CategoryId == 12).ToList();
      ViewBag.CategoryList = new SelectList(filtercategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductCTL/CreateLoiThepCTL.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProductLT(LoiThepCTLDTO product)
    {
      if (ModelState.IsValid)
      {
        await _productLTCTLService.CreateProductAsync(product);
        return RedirectToAction("ListLoiThepCTL");
      }

      // Nếu ModelState không hợp lệ, trả về view với lỗi
      var categories = await _productLTCTLService.GetCategories();
      var filtercategories = categories.Where(c => c.CategoryId == 12).ToList();
      ViewBag.CategoryList = new SelectList(filtercategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductCTL/CreateLoiThepCTL.cshtml", product);
    }
    public async Task<IActionResult> EditProductLT(int id)
    {
      var product = await _productLTCTLService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      var categories = await _productLTCTLService.GetCategories();
      var filterEditcategories = categories.Where(c => c.CategoryId == 12).ToList();
      ViewBag.CategoryList = new SelectList(filterEditcategories, "CategoryId", "CategoryName", product.CategoryId);
      return View("~/Views/ProductCTL/EditLoiThepCTL.cshtml", product); // Truyền product vào view
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductLT(LoiThepCTLDTO product)
    {
      try
      {
        await _productLTCTLService.UpdateProductAsync(product);
        return RedirectToAction(nameof(ListLoiThepCTL));
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", "Có Lỗi xảy ra khi cập nhật");
      }
      var categories = await _productLTCTLService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
      return View(product);
    }

    public async Task<IActionResult> ShowProductLTById(int id)
    {
      {
        var product = await _productLTCTLService.GetProductByIdAsync(id);
        if (product == null)
        {
          return NotFound();
        }
        return PartialView("~/Views/ProductCTL/ShowProductLTCTL.cshtml", product);
      }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProductLT(int id)
    {
      if (id <= 0)
      {
        return Json(new { success = false, message = "ID sản phẩm không hợp lệ." });
      }

      try
      {
        var product = await _productLTCTLService.GetProductByIdAsync(id);
        if (product == null)
        {
          return Json(new { success = false, message = "Sản phẩm không tồn tại." });
        }

        await _productLTCTLService.DeleteProductAsync(id);
        return Json(new { success = true, message = "Xóa sản phẩm thành công." });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi xóa sản phẩm: {ex.Message}" });
      }
    }
  }
}
