using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Controllers
{
  public class ProductCTLController : Controller
  {
    private readonly IProductCvCTLService _productCvCTLService;

    public ProductCTLController(IProductCvCTLService productCvCTLService)
    {
      _productCvCTLService = productCvCTLService;
    }

    public async Task<IActionResult> ListTieuChuanCTL(int page = 1, string searchName = null)
    {
      const int categoryId = 3;
      const int pageSize = 9;

      X.PagedList.IPagedList<ProductCTLDTO> products;
      if (!string.IsNullOrEmpty(searchName))
      {
        products = await _productCvCTLService.SearchProductsByNameAsync(searchName, categoryId, page, pageSize);
      }
      else
      {
        products = await _productCvCTLService.GetProducts(categoryId, page, pageSize);
      }
      return View("~/Views/ProductCTL/ListTieuChuanCTL.cshtml", products);
    }
    [HttpPost]
    public async Task<IActionResult> Search(string name, int page = 1)
    {
      const int categoryId = 3; // ID danh mục cố định
      const int pageSize = 9;

      var products = await _productCvCTLService.SearchProductsByNameAsync(name, categoryId, page, pageSize);
      return View("ListTieuChuanCTL", products);
    }
    public async Task<IActionResult> CreateProductCvCTL()
    {
      var categories = await _productCvCTLService.GetCategories();
      var filtercategories = categories.Where(c => c.CategoryId == 3).ToList();
      ViewBag.CategoryList = new SelectList(filtercategories, "CategoryId", "CategoryName");
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProductCvCTL(ProductCTLDTO product)
    {
      if (ModelState.IsValid)
      {
        // Lưu hình ảnh (nếu có) và thiết lập thuộc tính image
        if (product.imageFile != null)
        {
          var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", product.imageFile.FileName);
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await product.imageFile.CopyToAsync(stream);
          }
          product.image = product.imageFile.FileName;
        }

        // Gọi phương thức lưu từ service
        await _productCvCTLService.CreateProductAsync(product);
        return RedirectToAction("ListTieuChuanCTL");
      }

      var categories = await _productCvCTLService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
      return View(product);
    }
    public async Task<IActionResult> EditProductCTL(int id)
    {
      var product = await _productCvCTLService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      var categories = await _productCvCTLService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
      return View(product);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductCTL(ProductCTLDTO product)
    {
      if (ModelState.IsValid)
      {
        // Tải hình ảnh cũ từ cơ sở dữ liệu
        var existingProduct = await _productCvCTLService.GetProductByIdAsync(product.ProductId);
        if (existingProduct != null)
        {
          // Nếu không có hình ảnh mới thì giữ lại hình ảnh cũ
          if (product.imageFile == null)
          {
            product.image = existingProduct.image; // Giữ lại hình ảnh cũ
          }
          else
          {
            // Lưu hình ảnh mới
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", product.imageFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
              await product.imageFile.CopyToAsync(stream);
            }
            product.image = product.imageFile.FileName; // Cập nhật hình ảnh mới
          }
        }

        // Gọi phương thức lưu từ service
        await _productCvCTLService.UpdateProductAsync(product);
        return RedirectToAction("ListTieuChuanCTL");
      }

      var categories = await _productCvCTLService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
      return View(product);
    }
    public async Task<IActionResult> ShowProductCvCTLById(int id)
    {
      var product = await _productCvCTLService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductCTL/ProductModalCvCTL.cshtml", product);
    }
    [HttpPost]
    public async Task<IActionResult> DeleteProductCVCTL(int ProductId)
    {
      if (ProductId <= 0)
      {
        return BadRequest("Invalid Product ID.");
      }

      await _productCvCTLService.DeleteProductAsync(ProductId);
      return Ok();
    }
  }
}
