using AspnetCoreMvcFull.ModelDTO.Product;
using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspnetCoreMvcFull.Controllers
{
  public class GangCauCTLCotroller : Controller
  {
    private readonly IGangCauCTLService _gangCauCTLService;
    public GangCauCTLCotroller(IGangCauCTLService gangCauCTLService)
    {
      _gangCauCTLService = gangCauCTLService;
    }
    public async Task<IActionResult> ListGangCauCTL(int page = 1, string searchName = null)
    {
      const int categoryId = 8;
      const int pageSize = 9;
      X.PagedList.IPagedList<ProductGCCTLDTO> products;
      if (!string.IsNullOrEmpty(searchName))
      {
        products = await _gangCauCTLService.SearchProductsByNameAsync(searchName, categoryId, page, pageSize);
      }
      else
      {
        products = await _gangCauCTLService.GetProducts(categoryId, page, pageSize);
      }
      return View("~/Views/ProductCTL/ListGangCauCTL.cshtml", products);
    }
    public async Task<IActionResult> SearchGCCTL(string name, int page = 1)
    {
      const int categoryId = 8;
      const int PageSize = 9;
      var productsGC = await _gangCauCTLService.SearchProductsByNameAsync(name, categoryId, page, PageSize);
      return View("~/Views/ProductCTL/ListGangCauCTL.cshtml", productsGC);
    }
    public async Task<IActionResult> CreateGCCTL()
    {
      var categories = await _gangCauCTLService.GetCategories();
      var filterCategoriGC = categories.Where(c => c.CategoryId == 8).ToList().ToList();
      ViewBag.CategoryList = new SelectList(filterCategoriGC, "CategoryId", "CategoryName");
      return View("~/Views/ProductCTL/CreateGCCTL.cshtml");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGCCTL(ProductGCCTLDTO productGCCTLDTO)
    {
      if (ModelState.IsValid)
      {
        if (productGCCTLDTO.imageFile != null)
        {
          var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productGCCTLDTO.imageFile.FileName);
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await productGCCTLDTO.imageFile.CopyToAsync(stream);
          }
          productGCCTLDTO.image = productGCCTLDTO.imageFile.FileName;

        }
        await _gangCauCTLService.AddProductAsync(productGCCTLDTO);
        return RedirectToAction("ListGangCauCTL");
      }
      var categories = await _gangCauCTLService.GetCategories();
      var filterCategoriGC = categories.Where(c => c.CategoryId == 8).ToList().ToList();
      ViewBag.CategoryList = new SelectList(filterCategoriGC, "CategoryId", "CategoryName");
      return View(ListGangCauCTL);
    }
    public async Task<IActionResult> EditGCCTL(int id)
    {
      var editProductGC = await _gangCauCTLService.GetProductByIdAsync(id);
      if(editProductGC == null)
      {
        return NotFound();
      } else
      {
        var categories = await _gangCauCTLService.GetCategories();
        var filterCategoriGC = categories.Where(c => c.CategoryId == 8).ToList().ToList();
        ViewBag.CategoryList = new SelectList(filterCategoriGC, "CategoryId", "CategoryName");
        return View("~/Views/ProductCTL/EditGCCTL.cshtml", editProductGC);
      }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditGCCTL(ProductGCCTLDTO productGCCTLDTO)
    {
      if (!ModelState.IsValid)
      {
        var categories = await _gangCauCTLService.GetCategories();
        ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
        return View("~/Views/ProductCTL/EditProductCSCTL.cshtml", productGCCTLDTO);
      }

      var existingProduct = await _gangCauCTLService.GetProductByIdAsync(productGCCTLDTO.ProductId);

      if (productGCCTLDTO.imageFile != null && productGCCTLDTO.imageFile.Length > 0)
      {
        var fileName = Path.GetFileName(productGCCTLDTO.imageFile.FileName);
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
          productGCCTLDTO.image = newFileName;
        }
        else
        {
          productGCCTLDTO.image = fileName;
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await productGCCTLDTO.imageFile.CopyToAsync(stream);
        }
      }
      else
      {
        productGCCTLDTO.image = existingProduct.image;
      }

      await _gangCauCTLService.UpdateProductAsync(productGCCTLDTO);
      return RedirectToAction("ListGangCauCTL", "GangCauCTLCotroller");
    }
    public async Task<IActionResult> DeleteProductGCCTL (int ProductId)
    {
      if(ProductId <=0)
      {
        return BadRequest("Inavalid Product ID.");
      }
      await _gangCauCTLService.DeleteProductAsync(ProductId);
      return Ok();
    }
    public async Task<IActionResult> showProductGC(int id)
    {
      var productshow = await _gangCauCTLService.GetProductByIdAsync(id);
      if(productshow == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/ProductCTL/ModalGCCTL.cshtml", productshow);
    }
  }                                                       
}
