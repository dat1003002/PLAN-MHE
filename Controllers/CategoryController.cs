using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Service;
using PagedList;
using System;
using System.Threading.Tasks;

namespace AspnetCoreMvcFull.Controllers
{
  public class CategoryController : Controller
  {
    private readonly ICategoryService _categoryService;

    // Constructor để inject ICategoryService vào controller
    public CategoryController(ICategoryService categoryService)
    {
      _categoryService = categoryService;
    }

    // GET: /Category/Category
    // Hiển thị danh sách tất cả các danh mục
    public async Task<IActionResult> Category()
    {
      var categories = await _categoryService.GetAllCategoriesAsync();
      return View(categories);
    }

    public IActionResult AddCategory()
    {
      return View();
    }

    public IActionResult EditCategory()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(Category category)
    {
      if (!ModelState.IsValid)
      {
        // Nếu ModelState không hợp lệ, trả về View với thông báo lỗi
        return View(category);
      }

      try
      {
        // Cập nhật thuộc tính UpdatedAt nếu có
        category.UpdatedAt = DateTime.Now;

        // Thêm danh mục mới vào cơ sở dữ liệu
        await _categoryService.AddCategoryAsync(category);
        TempData["SuccessMessage"] = "Danh mục đã được thêm thành công!";
        return RedirectToAction("Category");
      }
      catch (Exception ex)
      {
        // Ghi lại lỗi và hiển thị thông báo cho người dùng
        ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi thêm danh mục: " + ex.Message);
        return View(category);
      }
    }
    public async Task<IActionResult> Pagination(int? page)
    {
      int pageSize = 10; // Số mục trên mỗi trang
      int pageNumber = page ?? 1; // Số trang hiện tại

      var categories = await _categoryService.GetAllCategoriesAsync();
      var pagedList = categories.ToPagedList(pageNumber, pageSize); // Chuyển đổi danh sách sang dạng phân trang

      return View(pagedList);
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
      var category = await _categoryService.GetCategoryByIdAsync(id);
      if (category == null)
      {
        return NotFound();
      }
      return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(Category category)
    {
      if (!ModelState.IsValid)
      {
        return View(category);
      }

      try
      {
        await _categoryService.UpdateCategoryAsync(category);
        TempData["SuccessMessage"] = "Danh mục đã được cập nhật thành công!";
        return RedirectToAction("Category");
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật danh mục: " + ex.Message);
        return View(category);
      }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
      try
      {
        await _categoryService.DeleteCategoryAsync(id);
        return Json(new { success = true });
      }
      catch (InvalidOperationException ex)
      {
        return Json(new { success = false, message = ex.Message });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
      }
    }
  }
}
