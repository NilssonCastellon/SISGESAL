using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISGESAL.Models;
using SISGESAL.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SISGESAL.Areas.Identity.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SISGESAL.Controllers
{
    [Authorize]
    public class ArticulosController : Controller
    {
        private readonly SisgesalContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArticulosController(SisgesalContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Articulos
        public async Task<IActionResult> Index()
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
                ViewBag.AlmacenMessage = "Debes crear un almacén antes de continuar.";
                return View(new List<ArticuloViewModel>()); // Retornar vista vacía si no hay almacén
            }

            Console.WriteLine($"Almacén asociado al usuario: {userAlmacen.Identificador}");

            // Obtener los artículos del almacén del usuario actual
            var articulos = await _context.Articulos
                .Where(t => t.CodigoAlmacen == userAlmacen.Identificador) // Filtrar por el almacén del usuario
                .Select(a => new ArticuloViewModel
                {
                    IdentificadorArticulo = a.IdentificadorArticulo,
                    CodigoArticulo = a.CodigoArticulo,
                    DescripcionArticulo = a.DescripcionArticulo,
                    TipoArticulo = a.TipoArticulo,
                    PuntoMinimo = a.PuntoMinimo,
                    PuntoMaximo = a.PuntoMaximo,
                    PuntoReorden = a.PuntoReorden,
                    UsuarioCreacion = a.UsuarioCreacion,
                    FechaCreacion = a.FechaCreacion,
                    Estado = a.Estado
                }).ToListAsync();

            return View(articulos);
        }


        // GET: Articulos/Create
        public IActionResult Create()
        {
            return PartialView("_CreateModal", new ArticuloViewModel());
        }

        // POST: Articulos/Create
        // POST: Articulos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticuloViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.Name;
                    var userAlmacen = await _context.Almacenes.FirstOrDefaultAsync(a => a.UsuarioCreacion == userName);
                    if (userAlmacen == null)
                    {
                        return Json(new { success = false, message = "No se encontró un almacén asociado al usuario." });
                    }

                    // Verifica que el CódigoAlmacen sea válido
                    if (userAlmacen.Identificador <= 0)
                    {
                        return Json(new { success = false, message = "El almacén asociado no tiene un código válido." });
                    }

                    // Crea el nuevo artículo
                    var articulo = new Articulo
                    {
                        CodigoArticulo = viewModel.CodigoArticulo,
                        DescripcionArticulo = viewModel.DescripcionArticulo,
                        TipoArticulo = viewModel.TipoArticulo,
                        PuntoMinimo = viewModel.PuntoMinimo,
                        PuntoMaximo = viewModel.PuntoMaximo,
                        PuntoReorden = viewModel.PuntoReorden,
                        UsuarioCreacion = userName,
                        FechaCreacion = DateTime.Now,
                        Estado = true,
                        CodigoAlmacen = userAlmacen.Identificador
                    };

                    // Añade el artículo a la base de datos
                    _context.Articulos.Add(articulo);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Artículo creado con éxito." });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Error al crear artículo: {ex.Message}",
                        stackTrace = ex.StackTrace,
                        exceptionType = ex.GetType().ToString()
                    });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, errors = errors });
        }







        // GET: Articulos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }

            var viewModel = new ArticuloViewModel
            {
                IdentificadorArticulo = articulo.IdentificadorArticulo,
                CodigoArticulo = articulo.CodigoArticulo,
                DescripcionArticulo = articulo.DescripcionArticulo,
                TipoArticulo = articulo.TipoArticulo,
                PuntoMinimo = articulo.PuntoMinimo,
                PuntoMaximo = articulo.PuntoMaximo,
                PuntoReorden = articulo.PuntoReorden,
            };

            return PartialView("_EditModal", viewModel);
        }

        // POST: Articulos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticuloViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var articulo = await _context.Articulos.FindAsync(viewModel.IdentificadorArticulo);
                if (articulo == null)
                {
                    return NotFound();
                }

                articulo.CodigoArticulo = viewModel.CodigoArticulo;
                articulo.DescripcionArticulo = viewModel.DescripcionArticulo;
                articulo.TipoArticulo = viewModel.TipoArticulo;
                articulo.PuntoMinimo = viewModel.PuntoMinimo;
                articulo.PuntoMaximo = viewModel.PuntoMaximo;
                articulo.PuntoReorden = viewModel.PuntoReorden;

                try
                {
                    _context.Update(articulo);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Artículo actualizado con éxito." });
                }
                catch (Exception ex)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, errors = errors });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors = errors });
            }
        }

        // POST: Articulos/ChangeStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }

            articulo.Estado = !articulo.Estado;

            try
            {
                _context.Update(articulo);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Estado cambiado con éxito." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cambiar el estado." });
            }
        }


        // Reportes vista Articulos
        public async Task<IActionResult> Report()
        {
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
                return View(new List<TransaccionViewModel>()); // Retornar vista vacía si no hay almacén
            }

            Console.WriteLine($"Almacén asociado al usuario: {userAlmacen.Identificador}");

            var articulos = await _context.Articulos
                .Where(t => t.CodigoAlmacen == userAlmacen.Identificador) // Filtrar por el almacén del usuario
                .Select(a => new ArticuloViewModel
                {
                    IdentificadorArticulo = a.IdentificadorArticulo,
                    CodigoArticulo = a.CodigoArticulo,
                    DescripcionArticulo = a.DescripcionArticulo,
                    TipoArticulo = a.TipoArticulo,
                    PuntoMinimo = a.PuntoMinimo,
                    PuntoMaximo = a.PuntoMaximo,
                    PuntoReorden = a.PuntoReorden,
                    UsuarioCreacion = a.UsuarioCreacion,
                    FechaCreacion = a.FechaCreacion,
                    Estado = a.Estado
                  

                }).ToListAsync();

            // Devuelve la vista "Report" en la carpeta de vistas del controlador
            return View("Report", articulos);
        }

    }
}
