using Microsoft.AspNetCore.Mvc;
using SISGESAL.Models.ViewModels;
using SISGESAL.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SISGESAL.Areas.Identity.Data;


namespace SISGESAL.Controllers
{
    [Authorize]
    public class TransaccionDetallesController : Controller
    {
        private readonly SisgesalContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransaccionDetallesController(SisgesalContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
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

                // Establecer el mensaje de error en ViewBag
                ViewBag.ErrorMessage = "Debes crear primero un almacén antes de ver los detalles de transacción.";

                // Retornar una vista vacía si no hay almacén
                return View(new List<TransaccionDetalleViewModel>());
            }

            Console.WriteLine($"Almacén asociado al usuario: {userAlmacen.Identificador}");

            // Obtener los detalles de la transacción del almacén del usuario actual
            var transaccionDetalles = await _context.TransaccionDetalles
                .Where(td => td.CodigoAlmacen == userAlmacen.Identificador) // Filtrar por el almacén del usuario
                .Select(td => new TransaccionDetalleViewModel
                {
                    IdentificadorTrans = td.IdentificadorTrans,
                    Ningresos = td.Ningresos,
                    CodigoAlmacen = td.CodigoAlmacen,
                    CodigoArticulo = td.CodigoArticulo,
                    Operacion = td.Operacion,
                    FechaIngreso = td.FechaIngreso,
                    Entrada = td.Entrada,
                    PrecioUnitario = td.PrecioUnitario,
                    CodigoAlmacenNavigation = new AlmaceneViewModel
                    {
                        Descripcion = _context.Almacenes
                            .Where(a => a.Identificador == td.CodigoAlmacen)
                            .Select(a => a.Descripcion)
                            .FirstOrDefault() ?? "Descripción no disponible"
                    },
                    CodigoArticuloNavigation = new ArticuloViewModel
                    {
                        DescripcionArticulo = _context.Articulos
                            .Where(a => a.IdentificadorArticulo == td.CodigoArticulo)
                            .Select(a => a.DescripcionArticulo)
                            .FirstOrDefault() ?? "Descripción no disponible"
                    }
                })
                .ToListAsync();

            // Imprimir el resultado de los detalles de la transacción
            if (!transaccionDetalles.Any())
            {
                Console.WriteLine("No se encontraron detalles de transacción para el almacén del usuario.");
            }
            else
            {
                Console.WriteLine($"Se encontraron {transaccionDetalles.Count} detalles de transacción para el almacén del usuario.");
            }

            return View(transaccionDetalles);
        }



        // Método Report 
        [HttpGet]
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

            // Obtener los detalles de transacciones
            var transaccionDetalles = await _context.TransaccionDetalles
                .AsNoTracking() // Añadir AsNoTracking para mejorar el rendimiento
                .Where(t => t.CodigoAlmacen == userAlmacen.Identificador) // Filtrar por el almacén del usuario
                .Select(td => new TransaccionDetalleViewModel
                {
                    IdentificadorTrans = td.IdentificadorTrans,
                    Ningresos = td.Ningresos,
                    CodigoAlmacen = td.CodigoAlmacen,
                    CodigoArticulo = td.CodigoArticulo,
                    Operacion = td.Operacion,
                    FechaIngreso = td.FechaIngreso,
                    Entrada = td.Entrada,
                    PrecioUnitario = td.PrecioUnitario,
                    CodigoAlmacenNavigation = new AlmaceneViewModel
                    {
                        Descripcion = _context.Almacenes
                            .Where(a => a.Identificador == td.CodigoAlmacen)
                            .Select(a => a.Descripcion)
                            .FirstOrDefault() ?? "Descripción no disponible"
                    },
                    CodigoArticuloNavigation = new ArticuloViewModel
                    {
                        DescripcionArticulo = _context.Articulos
                            .Where(a => a.IdentificadorArticulo == td.CodigoArticulo)
                            .Select(a => a.DescripcionArticulo)
                            .FirstOrDefault() ?? "Descripción no disponible"
                    }
                })
                .ToListAsync(); // Ejecutar la consulta de forma asíncrona

            // Retornar la vista con los detalles de transacción
            return View("Report", transaccionDetalles); // Asegúrate de usar 'transaccionDetalles'
        }

    }
}
