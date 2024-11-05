using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SISGESAL.Areas.Identity.Data;
using SISGESAL.Models;
using System.Diagnostics;

namespace SISGESAL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            //ViewData["UserID"]=_userManager.GetUserId(this.User);
            //return View();
            // Redirigir a la acción del Dashboard
            return RedirectToAction("Index", "Dashboard"); // Cambia "Index" y "Dashboard" según corresponda
        }

        /*
        public IActionResult Privacy()
        {
            return View();
        } */

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
