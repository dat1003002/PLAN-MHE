using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspnetCoreMvcFull.Controllers
{
  public class ThanHinhCTLCotronller : Controller
  {
    private readonly ILuuHoaCTLSevice _luuHoaCTLSevice;
    public ThanHinhCTLCotronller(ILuuHoaCTLSevice luuHoaCTLSevice)
    {
      _luuHoaCTLSevice = luuHoaCTLSevice;
    }

    public async Task<IActionResult> ListThanhHinhCTL(int page = 1, string searchName = null)
    {
      const int categoryId = 9;
      const int pageSize = 9;
      X.PagedList.IPagedList<LuuHoaCTLDTO> products;
      if (!string.IsNullOrEmpty(searchName))
      {
        products = await _luuHoaCTLSevice.SearchProductsByNameAsync(searchName, categoryId, page, pageSize);
      }
      else
      {
        products = await _luuHoaCTLSevice.GetProducts(categoryId, page, pageSize);
      }
      return View("~/Views/ProductCTL/ListThanhHinhCTL.cshtml",products);
    }
    public async Task<IActionResult> SearchLHTHCTL(string name, int page = 1)
    {
      const int categoryId = 9;
      const int PageSize = 9;
      var productLHTHCTL = await _luuHoaCTLSevice.SearchProductsByNameAsync(name, categoryId, page, PageSize);
      return View("~/Views/ProductCTL/ListThanhHinhCTL.cshtml", productLHTHCTL);
    }
    public async Task<IActionResult> CreateLHTHCTL()
    {
      var categories = await _luuHoaCTLSevice.GetCategories();
      var filterCategoriGC = categories.Where(c => c.CategoryId == 9).ToList().ToList();
      ViewBag.CategoryList = new SelectList(filterCategoriGC, "CategoryId", "CategoryName");
      return View("~/Views/ProductCTL/CreateLHTHCTL.cshtml");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLHTHCTL(LuuHoaCTLDTO luuHoaCTLDTO)
    {
      if (ModelState.IsValid)
      {
        if (luuHoaCTLDTO.imageFile != null)
        {
          var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", luuHoaCTLDTO.imageFile.FileName);
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await luuHoaCTLDTO.imageFile.CopyToAsync(stream);
          }
          luuHoaCTLDTO.image = luuHoaCTLDTO.imageFile.FileName;

        }
        await _luuHoaCTLSevice.AddProductAsync(luuHoaCTLDTO);
        return RedirectToAction("ListThanhHinhCTL");
      }
      var categories = await _luuHoaCTLSevice.GetCategories();
      var filterCategoriGC = categories.Where(c => c.CategoryId == 9).ToList().ToList();
      ViewBag.CategoryList = new SelectList(filterCategoriGC, "CategoryId", "CategoryName");
      return View(ListThanhHinhCTL);
    }
    public async Task<IActionResult> EditTHLHCTL(int id)
    {
      var editProductTHLHCTL = await _luuHoaCTLSevice.GetProductByIdAsync(id);
      if (editProductTHLHCTL == null)
      {
        return NotFound();
      }
      else
      {
        var categories = await _luuHoaCTLSevice.GetCategories();
        var filterCategoriGC = categories.Where(c => c.CategoryId == 9).ToList().ToList();
        ViewBag.CategoryList = new SelectList(filterCategoriGC, "CategoryId", "CategoryName");
        return View("~/Views/ProductCTL/EditTHLHCTL.cshtml", editProductTHLHCTL);
      }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTHLHCTL(LuuHoaCTLDTO luuHoaCTLDTO)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _luuHoaCTLSevice.GetCategories();
        ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
        return View("~/Views/ProductCTL/EditTHLHCTL.cshtml", luuHoaCTLDTO);
      }

      var existingProduct = await _luuHoaCTLSevice.GetProductByIdAsync(luuHoaCTLDTO.ProductId);

      if (luuHoaCTLDTO.imageFile != null && luuHoaCTLDTO.imageFile.Length > 0)
      {
        var fileName = Path.GetFileName(luuHoaCTLDTO.imageFile.FileName);
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
          luuHoaCTLDTO.image = newFileName;
        }
        else
        {
          luuHoaCTLDTO.image = fileName;
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await luuHoaCTLDTO.imageFile.CopyToAsync(stream);
        }
      }
      else
      {
        luuHoaCTLDTO.image = existingProduct.image;
      }

      await _luuHoaCTLSevice.UpdateProductAsync(luuHoaCTLDTO);
      return RedirectToAction("ListThanhHinhCTL", "ThanHinhCTLCotronller");
    }
    public async Task<IActionResult> DeleteProductLHTHCTL(int ProductId)
    {
      if (ProductId <= 0)
      {
        return BadRequest("Inavalid Product ID.");
      }
      await _luuHoaCTLSevice.DeleteProductAsync(ProductId);
      return Ok();
    }
    public async Task<IActionResult> showProductLHTHCTL(int id)
    {
      var productshow = await _luuHoaCTLSevice.GetProductByIdAsync(id);
      if (productshow == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductCTL/ModalLuuHoaCTL.cshtml", productshow);
    }
  }
}
