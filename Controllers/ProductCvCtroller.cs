using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList; // Nếu bạn không cần sử dụng
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList; // Nếu bạn cần sử dụng

namespace AspnetCoreMvcFull.Controllers
{
  public class ProductCvController : Controller
  {
    private readonly IProductCvService _productCvService;

    public ProductCvController(IProductCvService productCvService)
    {
      _productCvService = productCvService;
    }
    public async Task<IActionResult> ListTieuChuanCV(int page = 1, string searchName = null)
    {
      const int categoryId = 2; // Lấy sản phẩm có categoryId = 2
      const int pageSize = 10; // Số sản phẩm hiển thị trên mỗi trang

      // Xác định rõ ràng loại IPagedList
      X.PagedList.IPagedList<ProductDTO> products;
      if (!string.IsNullOrEmpty(searchName))
      {
        products = await _productCvService.SearchProductsByNameAsync(searchName, categoryId, page, pageSize);
      }
      else
      {
        products = await _productCvService.GetProducts(categoryId, page, pageSize);
      }
      // Trả về view với danh sách sản phẩm đã lấy được
      return View("~/Views/ProductMhe/ListTieuChuanCV.cshtml", products);
    }
    [HttpPost]
    public async Task<IActionResult> Search(string name, int page = 1)
    {
      const int categoryId = 2; // ID danh mục cố định
      const int pageSize = 10; // Số sản phẩm hiển thị trên mỗi trang

      var products = await _productCvService.SearchProductsByNameAsync(name, categoryId, page, pageSize);

      // Trả về view với danh sách sản phẩm tìm kiếm được
      return View("~/Views/ProductMhe/ListTieuChuanCV.cshtml", products);
    }
    public async Task<IActionResult> CreateProductCV()
    {
      var categories = await _productCvService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
      return View("~/Views/ProductMhe/CreateProductCV.cshtml");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProductCV(ProductDTO productDTO)
    {
      if (ModelState.IsValid)
      {
        if (productDTO.imageFile != null && productDTO.imageFile.Length > 0)
        {
          var fileName = Path.GetFileName(productDTO.imageFile.FileName);
          var directoryPath = Path.Combine("wwwroot/images");
          if (!Directory.Exists(directoryPath))
          {
            Directory.CreateDirectory(directoryPath); // Tạo thư mục nếu không tồn tại
          }
          var filePath = Path.Combine(directoryPath, fileName); // Thay đổi đường dẫn theo ý bạn

          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await productDTO.imageFile.CopyToAsync(stream); // Lưu file ảnh
          }

          // Lưu tên file vào productDTO.image
          productDTO.image = fileName; // Lưu tên file vào thuộc tính string
        }

        await _productCvService.AddProductAsync(productDTO);
        return RedirectToAction("ListTieuChuanCV"); // Chuyển hướng về danh sách sản phẩm
      }

      var categories = await _productCvService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", productDTO.CategoryId);
      return View("~/Views/ProductMhe/CreateProductCV.cshtml", productDTO);
    }
    public async Task<IActionResult> EditProductCV(int id)
{
    var product = await _productCvService.GetProductByIdAsync(id);
    if (product == null)
    {
        return NotFound();
    }

    var categories = await _productCvService.GetCategories();
    ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
    return View("~/Views/ProductMhe/EditProductCV.cshtml", product);
}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProductCV(ProductDTO productDTO)
    {
      if (ModelState.IsValid)
      {
        var existingProduct = await _productCvService.GetProductByIdAsync(productDTO.ProductId);
        if (existingProduct == null)
        {
          return NotFound();
        }

        // Kiểm tra nếu người dùng có chọn ảnh mới không
        if (productDTO.imageFile != null && productDTO.imageFile.Length > 0)
        {
          // Xóa ảnh cũ nếu cần
          var oldFilePath = Path.Combine("wwwroot/images", existingProduct.image);
          if (System.IO.File.Exists(oldFilePath))
          {
            System.IO.File.Delete(oldFilePath);
          }

          var fileName = Path.GetFileName(productDTO.imageFile.FileName);
          var filePath = Path.Combine("wwwroot/images", fileName);

          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await productDTO.imageFile.CopyToAsync(stream);
          }

          productDTO.image = fileName; // Cập nhật tên file mới
        }
        else
        {
          productDTO.image = existingProduct.image; // Giữ nguyên ảnh cũ
        }

        await _productCvService.UpdateProductAsync(productDTO);
        return RedirectToAction("ListTieuChuanCV"); // Chuyển hướng về danh sách sản phẩm
      }

      var categories = await _productCvService.GetCategories();
      ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName", productDTO.CategoryId);
      return View("~/Views/ProductMhe/EditProductCV.cshtml", productDTO);
    }
    [HttpPost]
    public async Task<IActionResult> DeleteProductCV(int ProductId)
    {
      if (ProductId <= 0)
      {
        return BadRequest("Invalid Product ID.");
      }

      await _productCvService.DeleteProductAsync(ProductId);
      return Ok();
    }
    public async Task<IActionResult> ShowProductCvById(int id)
    {
      var product = await _productCvService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductMhe/ProductModalCv.cshtml", product);
    }
  }
}
