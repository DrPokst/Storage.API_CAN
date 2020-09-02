using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Storage.API
{
    public class FallbackController : Controller
    {
        public IActionResult Index ()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");
        }
    }
}