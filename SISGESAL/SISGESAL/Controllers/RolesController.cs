using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISGESAL.Areas.Identity.Data;
using SISGESAL.Models;
using SISGESAL.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class UserRoleController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync(); // Obtiene todos los usuarios
        var userRoles = new List<UserRoleViewModel>(); // Lista para almacenar usuarios y sus roles

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user); // Obtiene roles para el usuario
            userRoles.Add(new UserRoleViewModel
            {
                User = user,
                Roles = roles.ToList()
            });
        }

        return View(userRoles); // Pasa la lista de usuarios y roles a la vista
    }


    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var roles = _roleManager.Roles.ToList();
        var userRoles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = roles;
        ViewBag.UserRoles = userRoles;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, string[] selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(id);
        var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);
        await _userManager.AddToRolesAsync(user, selectedRoles);
        return RedirectToAction(nameof(Index));
    }
}
