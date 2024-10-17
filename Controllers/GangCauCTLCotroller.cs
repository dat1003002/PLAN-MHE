using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers
{
  public class GangCauCTLCotroller : Controller
  {
    public async Task<IActionResult> ListGangCauCTL()
    {
      return View("~/Views/ProductCTL/ListGangCauCTL.cshtml");
    }
  }
}
