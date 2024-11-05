using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SISGESAL.Areas.Identity.Data;
using SISGESAL.Data;
using SISGESAL.Models;
using SISGESAL.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SISGESAL.Controllers
{
    [Authorize]
    public class ConfiguraAlmacenArticuloController : Controller
    {
        private readonly SisgesalContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfiguraAlmacenArticuloController(SisgesalContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener el usuario actual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirigir si no está autenticado
            }

            var username = currentUser.UserName;
            Console.WriteLine($"Nombre de usuario actual: {username}");

            // Obtener el almacén del usuario actual
            var userAlmacen = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioCreacion == username);

            // Verificar si no existe un almacén asociado al usuario
            if (userAlmacen == null)
            {
                ViewBag.ErrorMessage = "No tienes un almacén asociado. Crea uno antes de continuar.";
                ViewBag.Almacenes = new SelectList(new List<Almacene>());
                ViewBag.Articulos = new SelectList(new List<Articulo>());
                return View(new List<ConfiguraAlmacenArticuloViewModel>()); // Retorna vista vacía
            }

            Console.WriteLine($"Almacén asociado al usuario: {userAlmacen.Identificador}");

            // Cargar listas de Almacenes y Articulos según disponibilidad
            ViewBag.Almacenes = _context.Almacenes.Any()
                ? new SelectList(_context.Almacenes, "Identificador", "Descripcion", userAlmacen.Identificador)
                : new SelectList(new List<Almacene>());

            ViewBag.Articulos = _context.Articulos.Any(a => a.CodigoAlmacen == userAlmacen.Identificador)
                ? new SelectList(_context.Articulos.Where(a => a.CodigoAlmacen == userAlmacen.Identificador), "IdentificadorArticulo", "DescripcionArticulo")
                : new SelectList(new List<Articulo>());

            // Obtener la configuración del almacén y artículos
            var model = await _context.ConfiguraAlmacenArticulos
                .Where(c => c.IdentificadorAlmacen == userAlmacen.Identificador) // Filtrar por almacén
                .Select(c => new ConfiguraAlmacenArticuloViewModel
                {
                    IdentificadorConfig = c.IdentificadorConfig,
                    IdentificadorAlmacen = c.IdentificadorAlmacen,
                    IdentificadorArticulo = c.IdentificadorArticulo,
                    Estado = c.Estado,
                    AlmacenDescripcion = c.IdentificadorAlmacenNavigation.Descripcion,
                    ArticuloDescripcion = c.IdentificadorArticuloNavigation.DescripcionArticulo
                }).ToListAsync();

            // Pasar la descripción del almacén y su ID a la vista
            ViewBag.AlmacenDescripcion = userAlmacen.Descripcion;
            ViewBag.AlmacenId = userAlmacen.Identificador;

            return View(model);
        }




        //Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfiguraAlmacenArticuloViewModel model)
        {
            // Obtener el usuario actual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            // Obtener el almacén del usuario actual
            var userAlmacen = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioCreacion == currentUser.UserName);

            if (userAlmacen == null)
            {
                return Json(new { success = false, message = "El usuario no tiene un almacén asociado." });
            }

            if (!ModelState.IsValid)
            {
                var newConfig = new ConfiguraAlmacenArticulo
                {
                    IdentificadorAlmacen = userAlmacen.Identificador, // Usar el almacén del usuario
                    IdentificadorArticulo = model.IdentificadorArticulo,
                    Estado = true
                };

                _context.ConfiguraAlmacenArticulos.Add(newConfig);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Configuración creada exitosamente." });
            }

            return Json(new { success = false, message = "Hay errores en el formulario." });
        }



        // GET: ConfiguraAlmacenArticulo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var config = await _context.ConfiguraAlmacenArticulos
                .Include(c => c.IdentificadorAlmacenNavigation) // Incluye el almacén
                .FirstOrDefaultAsync(c => c.IdentificadorConfig == id);

            if (config == null)
            {
                return NotFound();
            }

            var viewModel = new ConfiguraAlmacenArticuloViewModel
            {
                IdentificadorConfig = config.IdentificadorConfig,
                IdentificadorArticulo = config.IdentificadorArticulo,
                AlmacenDescripcion = config.IdentificadorAlmacenNavigation.Descripcion, // Asigna la descripción del almacén
                IdentificadorAlmacen = config.IdentificadorAlmacen // Este campo puede ser necesario para mantener la relación
            };

            return PartialView("_Edit", viewModel); // Devuelve una vista parcial con el viewModel
        }


        // POST: ConfiguraAlmacenArticulo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConfiguraAlmacenArticuloViewModel model)
        {
            // Obtener el usuario actual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado." });
            }

            // Obtener el almacén del usuario actual
            var userAlmacen = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioCreacion == currentUser.UserName);

            if (userAlmacen == null)
            {
                return Json(new { success = false, message = "El usuario no tiene un almacén asociado." });
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var config = await _context.ConfiguraAlmacenArticulos.FindAsync(model.IdentificadorConfig);
                    if (config == null)
                    {
                        return Json(new { success = false, message = "Configuración no encontrada." });
                    }

                    // No permitir modificar el almacén
                    config.IdentificadorArticulo = model.IdentificadorArticulo;

                    _context.ConfiguraAlmacenArticulos.Update(config);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Configuración actualizada exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Error de validación." });
        }




        // GET: Report
        public async Task<IActionResult> Report(int? almacenId)
        {
            // Obtener el usuario actual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirigir al usuario a la página de login si no está autenticado
            }

            // Obtener el nombre de usuario
            var username = currentUser.UserName;
            Console.WriteLine($"Nombre de usuario actual: {username}");

            // Obtener el almacén del usuario actual
            var userAlmacen = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioCreacion == username);

            if (userAlmacen == null)
            {
                Console.WriteLine("El usuario no tiene un almacén asociado.");
                return View(new List<ConfiguraAlmacenArticuloViewModel>()); // Retornar vista vacía si no hay almacén
            }

            Console.WriteLine($"Almacén asociado al usuario: {userAlmacen.Identificador}");

            // Filtrar artículos según el almacén del usuario
            ViewBag.Almacenes = new SelectList(_context.Almacenes, "Identificador", "Descripcion", userAlmacen.Identificador);
            ViewBag.Articulos = new SelectList(
                _context.Articulos.Where(a => a.CodigoAlmacen == userAlmacen.Identificador), // Filtrar por almacén
                "IdentificadorArticulo",
                "DescripcionArticulo");

            // Filtrar configuraciones por el almacén del usuario o por el ID proporcionado
            var configuraciones = await _context.ConfiguraAlmacenArticulos
                .Include(c => c.IdentificadorAlmacenNavigation)
                .Include(c => c.IdentificadorArticuloNavigation)
                .Where(c => c.IdentificadorAlmacen == userAlmacen.Identificador &&
                             (!almacenId.HasValue || c.IdentificadorAlmacen == almacenId)) // Filtrado por almacén
                .Select(c => new ConfiguraAlmacenArticuloViewModel
                {
                    IdentificadorConfig = c.IdentificadorConfig,
                    IdentificadorAlmacen = c.IdentificadorAlmacen,
                    IdentificadorArticulo = c.IdentificadorArticulo,
                    Estado = c.Estado,
                    AlmacenDescripcion = c.IdentificadorAlmacenNavigation.Descripcion,
                    ArticuloDescripcion = c.IdentificadorArticuloNavigation.DescripcionArticulo
                }).ToListAsync();

            // Pasar la descripción del almacén y su ID a la vista
            ViewBag.AlmacenDescripcion = userAlmacen.Descripcion; // Descripción del almacén
            ViewBag.AlmacenId = userAlmacen.Identificador; // ID del almacén

            return View("Report", configuraciones);
        }

    }
}