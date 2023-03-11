using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace dotnet7.Controllers;

[Tags("Features")]
public class GatedController : Controller
{
    [HttpGet]
    [Route("/fm/gated/{message}")]
    [FeatureGate("PLAIN.KEYB")] // no context with a gate
    [Produces("application/json")]
    public ActionResult<string> Index(string? message)
    {
        return "This is gated "+message;
    }
}
