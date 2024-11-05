using Microsoft.AspNetCore.Mvc;
using SISGESAL.Data;
using SISGESAL.Models.ViewModels; // Asegúrate de que el namespace sea correcto
using SISGESAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SISGESAL.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace SISGESAL.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly SisgesalContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(SisgesalContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            // Obtener el usuario actual
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirigir si el usuario no está autenticado
            }

            // Obtener el nombre de usuario
            var username = currentUser.UserName;

            // Obtener el almacén del usuario actual
            var userAlmacen = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioCreacion == username);

            if (userAlmacen == null)
            {
                return View(new DashboardViewModel
                {
                    TotalArticulos = 0,
                    ArticulosBajoStock = new List<Articulo>(),
                    TotalTransacciones = 0,
                    NombreAlmacen = "No asignado"
                }); // Retornar vista vacía si no hay almacén
            }

            // Filtrar los artículos bajo stock
            var articulosBajoStockQuery = _context.Articulos
                .Where(a => a.PuntoReorden > 0 && a.Estado == true && a.CodigoAlmacen == userAlmacen.Identificador)
                .OrderBy(a => a.CodigoArticulo);

            // Obtener el total de artículos bajo stock
            var totalArticulosBajoStock = await articulosBajoStockQuery.CountAsync();

            // Aplicar la paginación
            var articulosBajoStock = await articulosBajoStockQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Calcular el total de páginas
            int totalPages = (int)Math.Ceiling(totalArticulosBajoStock / (double)pageSize);

            // Obtener el número de transacciones realizadas en el almacén del usuario
            var numeroTransacciones = await _context.Transaccions
                .Where(t => t.CodigoAlmacen == userAlmacen.Identificador)
                .CountAsync();

            // Crear el ViewModel con los datos
            var viewModel = new DashboardViewModel
            {
                TotalArticulos = totalArticulosBajoStock,
                ArticulosBajoStock = articulosBajoStock,
                TotalTransacciones = numeroTransacciones,
                NombreAlmacen = userAlmacen.Descripcion
            };

            ViewData["UserID"] = _userManager.GetUserId(this.User);
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = pageNumber;

            return View(viewModel);
        }

    }
}
