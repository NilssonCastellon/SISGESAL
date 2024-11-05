using Microsoft.AspNetCore.Mvc;
using SISGESAL.Models;
using SISGESAL.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using SISGESAL.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace SISGESAL.Controllers
{
    [Authorize]
    public class AspNetUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AspNetUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: AspNetUsers
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            var userViewModels = new List<AspNetUserViewModel>();

            foreach (var user in users)
            {
                userViewModels.Add(new AspNetUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount
                });
            }

            return View(userViewModels);
        }

        // GET: AspNetUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new AspNetUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount
            };

            return View(viewModel);
        }

        // GET: AspNetUsers/Create
        public IActionResult Create()
        {
            return View();
        }
        /*
        // POST: AspNetUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AspNetUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new AspNetUser
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    EmailConfirmed = viewModel.EmailConfirmed,
                    LockoutEnabled = viewModel.LockoutEnabled,
                    AccessFailedCount = viewModel.AccessFailedCount
                };

                var result = await _userManager.CreateAsync(user, "DefaultPassword123!");

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);
        }

        // GET: AspNetUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new AspNetUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount
            };

            return View(viewModel);
        }

        // POST: AspNetUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AspNetUserViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = viewModel.UserName;
                user.Email = viewModel.Email;
                user.EmailConfirmed = viewModel.EmailConfirmed;
                user.LockoutEnabled = viewModel.LockoutEnabled;
                user.AccessFailedCount = viewModel.AccessFailedCount;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);
        }
        */
    }
        
}
