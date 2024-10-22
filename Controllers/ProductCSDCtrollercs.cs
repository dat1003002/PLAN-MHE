using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace AspnetCoreMvcFull.Controllers
{
  public class ProductCSDCtrollercs : Controller
  {
    private readonly IProductCSDService _productCSDService;
    public ProductCSDCtrollercs(IProductCSDService productCSDService)
    {
      _productCSDService = productCSDService;
    }

    public async Task<IActionResult> ListCaoSuDun()
    {
      int categoryId = 5;

      var products = await _productCSDService.GetProducts(categoryId);
      return View("~/Views/ProductMhe/ListCaoSuDun.cshtml", products);
    }
    public async Task<IActionResult> CreateProductCSD()
    {
      var categories = await _productCSDService.GetCategories();
      var filtercategories = categories.Where(c => c.CategoryId == 5 || c.CategoryId == 6).ToList();
      ViewBag.CategoryList = new SelectList(filtercategories, "CategoryId", "CategoryName");
      return View("~/Views/ProductMhe/CreateProductCSD200.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProductCSD(ProductCSDDTO product)
    {
      if (ModelState.IsValid)
      {
        await _productCSDService.CreateProductAsync(product);
        return RedirectToAction("ListCaoSuDun"); 
      }

      var categories = await _productCSDService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
      return View("~/Views/ProductMhe/CreateProductCSD200.cshtml",product);
    }
    public async Task<IActionResult> EditProductCSD(int id)
    {
      var product = await _productCSDService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      var categories = await _productCSDService.GetCategories();
      var filterEditcategories = categories.Where(c => c.CategoryId == 5 || c.CategoryId == 6).ToList();
      ViewBag.CategoryList = new SelectList(filterEditcategories, "CategoryId", "CategoryName");

      return View("~/Views/ProductMhe/EditProductCSD.cshtml", product);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductCSD (ProductCSDDTO productCSDDTO)
    {
      try {
        await _productCSDService.UpdateProductCSDAsync(productCSDDTO);
        return RedirectToAction(nameof(ListCaoSuDun));
      } catch (Exception ex)
      {
        ModelState.AddModelError("", "Có Lỗi xảy ra khi cập nhật");
      }
      var categories = await _productCSDService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", productCSDDTO.CategoryId);
      return View(productCSDDTO);
    }
    public async Task<IActionResult> DeleteProductCSD(int ProductId)
    {
      if (ProductId <= 0)
      {
        return BadRequest("Invalid Product ID.");
      }
      await _productCSDService.DeleteProductAsync (ProductId);
      return Ok();
    }
    public async Task<IActionResult> showProductCSDById(int id)
    {
      var product = await _productCSDService.GetProductByIdAsync(id);
      if (product == null) 
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductMhe/ProductModalCSD.cshtml", product);
    }
    public async Task<IActionResult> ListCaoSuDun250()
    {
      const int categoryId = 6;

       IEnumerable<ProductCSDDTO> products = await _productCSDService.GetProducts(categoryId);
      return View("~/Views/ProductMhe/ListCaoSuDun250.cshtml", products);
    }

  }
}
