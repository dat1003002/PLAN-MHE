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
  public class QuyCachCaoSuCTLController : Controller
  {
    private readonly IQuyCachCSCTLService _quyCachCSCTLService;

    public QuyCachCaoSuCTLController(IQuyCachCSCTLService quyCachCSCTLService)
    {
      _quyCachCSCTLService = quyCachCSCTLService;
    }

    public async Task<IActionResult> ListQuyCachCSDCTL()
    {
      int categoryId = 13;
      var products = await _quyCachCSCTLService.GetProducts(categoryId);
      return View("~/Views/ProductCTL/ListQuyCachCSDCTL.cshtml", products);
    }
    public async Task<IActionResult> CreateProduct()
    {
      var categories = await _quyCachCSCTLService.GetCategories();
      var filtercategories = categories.Where(c => c.CategoryId == 13).ToList();
      ViewBag.CategoryList = new SelectList(filtercategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductCTL/CreateQuyCachCSDCTL.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(QuyCachCaoSuCTLDTO product)
    {
      if (ModelState.IsValid)
      {
        await _quyCachCSCTLService.CreateProductAsync(product);
        return RedirectToAction("ListQuyCachCSDCTL");
      }
      var categories = await _quyCachCSCTLService.GetCategories();
      var filtercategories = categories.Where(c => c.CategoryId == 13).ToList();
      ViewBag.CategoryList = new SelectList(filtercategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductCTL/CreateQuyCachCSDCTL.cshtml", product);
    }
    public async Task<IActionResult> EditProduct(int id)
    {
      var product = await _quyCachCSCTLService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      var categories = await _quyCachCSCTLService.GetCategories();
      var filterEditcategories = categories.Where(c => c.CategoryId == 13).ToList();
      ViewBag.CategoryList = new SelectList(filterEditcategories, "CategoryId", "CategoryName", product.CategoryId);
      return View("~/Views/ProductCTL/EditQuyCachCSDCTL.cshtml", product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(QuyCachCaoSuCTLDTO product)
    {
      try
      {
        await _quyCachCSCTLService.UpdateProductAsync(product);
        return RedirectToAction(nameof(ListQuyCachCSDCTL));
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", "Có Lỗi xảy ra khi cập nhật");
      }
      var categories = await _quyCachCSCTLService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
      return View(product);
    }

    public async Task<IActionResult> ShowProductById(int id)
    {
      {
        var product = await _quyCachCSCTLService.GetProductByIdAsync(id);
        if (product == null)
        {
          return NotFound();
        }
        return PartialView("~/Views/ProductCTL/ShowQuyCachCSDCTL.cshtml", product);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
      if (id <= 0)
      {
        return Json(new { success = false, message = "ID sản phẩm không hợp lệ." });
      }

      try
      {
        var product = await _quyCachCSCTLService.GetProductByIdAsync(id);
        if (product == null)
        {
          return Json(new { success = false, message = "Sản phẩm không tồn tại." });
        }

        await _quyCachCSCTLService.DeleteProductAsync(id);
        return Json(new { success = true, message = "Xóa sản phẩm thành công." });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = $"Lỗi khi xóa sản phẩm: {ex.Message}" });
      }
    }
  }
}
