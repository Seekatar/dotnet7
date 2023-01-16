using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace dotnet7.Controllers
{
    public class GatedController : Controller
    {
        [HttpGet]
        [Route("/gated")]
        [FeatureGate("CNTXT.KEYB")]
        public ActionResult<string> Index()
        {
            return "This is gated";
        }
    }
}
