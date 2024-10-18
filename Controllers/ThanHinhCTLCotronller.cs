using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers
{
  public class ThanHinhCTLCotronller : Controller
  {
    public IActionResult ListThanhHinhCTL()
    {
      return View("~/Views/ProductCTL/ListThanhHinhCTL.cshtml");
    }
  }
}
