using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISGESAL.Models;
using SISGESAL.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SISGESAL.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace SISGESAL.Controllers
{
    [Authorize]
    public class TransaccionesController : Controller
    {
        private readonly SisgesalContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CodigoGenerator _codigoGenerator;
        private readonly RequisicionService _requisicionService;

        public TransaccionesController(SisgesalContext context, UserManager<ApplicationUser> userManager, 
            CodigoGenerator codigoGenerator, RequisicionService requisicionService)
        {
            _context = context;
            _userManager = userManager;
            _codigoGenerator = codigoGenerator;
            _codigoGenerator = codigoGenerator;
            _requisicionService = requisicionService;
        }

        private async Task<Almacene> ObtenerAlmacenPredeterminadoAsync()
        {
            // Supongamos que hay un criterio para seleccionar el almacén predeterminado
            return await _context.Almacenes
                .OrderBy(a => a.Identificador) // Puedes cambiar el criterio según tus necesidades
                .FirstOrDefaultAsync();
        }


        // Método para obtener el código generado
        [HttpGet]
        public string GenerarCodigo(string codigoAlmacen)
        {
            if (int.TryParse(codigoAlmacen, out int identificador))
            {
                try
                {
                    var nuevoCodigo = _codigoGenerator.GenerarCodigo(identificador);
                    return nuevoCodigo; // Devuelve el código como string
                }
                catch (Exception ex)
                {
                    // Puedes manejar el error o lanzar una excepción
                    throw new Exception("Error al generar el código: " + ex.Message);
                }
            }
            else
            {
                throw new ArgumentException("Código de almacén inválido.");
            }
        }



        // Método para número de reuisición 
        public IActionResult CrearRequisicion()
        {
            int numeroRequisicion = _requisicionService.ObtenerProximoNumeroRequisicion();
            ViewBag.NumeroRequisicion = numeroRequisicion;
            return View();
        }


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
                ViewBag.ErrorMessage = "Debes crear primero un almacén antes de ver las transacciones.";

                // Retornar una vista vacía si no hay almacén
                return View(new List<TransaccionViewModel>());
            }

            Console.WriteLine($"Almacén asociado al usuario: {userAlmacen.Identificador}");

            // Obtener las transacciones del almacén del usuario actual
            var transacciones = await _context.Transaccions
                .Where(t => t.CodigoAlmacen == userAlmacen.Identificador) // Filtrar por el almacén del usuario
                .Select(t => new TransaccionViewModel
                {
                    Ningresos = t.Ningresos,
                    CodigoAlmacen = t.CodigoAlmacen,
                    Nrequisicion = t.Nrequisicion,
                    FechaRequisicion = t.FechaRequisicion,
                    Operacion = t.Operacion,
                    FechaIngreso = t.FechaIngreso,
                    Observacion = t.Observacion
                })
                .ToListAsync();

            // Imprimir el resultado de las transacciones
            if (!transacciones.Any())
            {
                Console.WriteLine("No se encontraron transacciones para el almacén del usuario.");
            }
            else
            {
                Console.WriteLine($"Se encontraron {transacciones.Count} transacciones para el almacén del usuario.");
            }

            return View(transacciones);
        }




        public async Task<int> ObtenerProximoNumeroRequisicionAsync()
        {
            var nuevoNumeroParam = new SqlParameter("@NuevoNumero", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC ObtenerProximoNumeroRequisicion @NuevoNumero OUTPUT", nuevoNumeroParam);
                return (int)nuevoNumeroParam.Value;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción aquí
                throw new InvalidOperationException("No se pudo obtener el próximo número de requisición.", ex);
            }
        }

        // GET: /Transacciones/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Asegúrate de que 'currentUser' no sea nulo
            if (currentUser == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Obtener el nombre de usuario
            var username = currentUser.UserName;
            Console.WriteLine($"Nombre de usuario actual: {username}");

            // Obtener el almacén del usuario actual
            var userAlmacen = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.UsuarioCreacion == username);

            // Verificar si se encontró un almacén
            if (userAlmacen == null)
            {
                Console.WriteLine("No se encontró un almacén asociado con el usuario.");
                TempData["AlmacenError"] = "No se encontró un almacén asociado al usuario.";
                return RedirectToAction("Index");
            }

            // Obtener la lista de almacenes
            var almacenes = await _context.Almacenes.ToListAsync();
            Console.WriteLine($"Cantidad de almacenes: {almacenes.Count}");
            foreach (var almacen in almacenes)
            {
                Console.WriteLine($"Almacén: {almacen.Identificador}, Usuario: {almacen.UsuarioCreacion}");
            }

            // Obtener la lista de artículos activos
            var articulos = await _context.Articulos
                .Where(a => a.Estado) // Filtra solo los artículos activos (true)
                .ToListAsync();

            var proximoNumeroRequisicion = _requisicionService.ObtenerProximoNumeroRequisicion();

            // Verifica si hay almacenes y artículos disponibles
            if (!almacenes.Any() || !articulos.Any())
            {
                return NotFound("No se encontraron almacenes o artículos.");
            }

            // Generar el código para el almacén del usuario
            string codigoAlmacen = userAlmacen.Identificador.ToString(); // Convertir a string
            string numeroIngresos;

            // Verifica si ya existe un número de ingresos en la sesión
            if (HttpContext.Session.TryGetValue("NumeroIngresos", out var numeroIngresosBytes))
            {
                numeroIngresos = Encoding.UTF8.GetString(numeroIngresosBytes);

                // Verifica si el almacén del usuario ha cambiado
                var almacenSession = HttpContext.Session.GetString("CodigoAlmacen");
                if (almacenSession != codigoAlmacen)
                {
                    // Si el almacén ha cambiado, generar un nuevo número de ingresos
                    numeroIngresos = GenerarCodigo(codigoAlmacen);
                    HttpContext.Session.SetString("NumeroIngresos", numeroIngresos);
                    HttpContext.Session.SetString("CodigoAlmacen", codigoAlmacen); // Actualiza el almacén en la sesión
                }
            }
            else
            {
                // Genera y guarda el número de ingresos en la sesión
                numeroIngresos = GenerarCodigo(codigoAlmacen);
                HttpContext.Session.SetString("NumeroIngresos", numeroIngresos);
                HttpContext.Session.SetString("CodigoAlmacen", codigoAlmacen); // Guarda el almacén en la sesión
            }

            // Aquí inicializamos la lista de detalles de la transacción
            var transaccionDetalles = new List<TransaccionDetalleViewModel>();
            // Crear el modelo para la vista
            var model = new TransaccionViewModel
            {
                Ningresos = numeroIngresos, // Aquí se usa el código generado
                CodigoAlmacen = userAlmacen.Identificador, // Aquí ya es un int
                Almacenes = almacenes,
                Articulos = articulos,
                AlmacenPredeterminado = userAlmacen,
                Nrequisicion = proximoNumeroRequisicion, // Número de Requisición
                TransaccionDetalles = transaccionDetalles  // Inicializa los detalles de la transacción
            };

            // Retorna la vista con el modelo
            return View(model);
        }



        // Método Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransaccionViewModel model)
        {
            try
            {
                // Log del valor de CodigoAlmacen
                Console.WriteLine($"Código Almacén recibido: {model.CodigoAlmacen}");

                // Verificar si el almacén existe
                var almacenExists = await _context.Almacenes
                    .AnyAsync(a => a.Identificador == model.CodigoAlmacen);

                if (!almacenExists)
                {
                    return Json(new { success = false, errorMessage = "El almacén especificado no existe." });
                }

                // Verificar si se ha seleccionado al menos un artículo
                if (model.SelectedArticulos == null || !model.SelectedArticulos.Any())
                {
                    return Json(new { success = false, errorMessage = "Debe seleccionar al menos un artículo para crear la transacción." });
                }

                // Obtener el siguiente número de requisición
                var nuevoNumeroParam = new SqlParameter("@NuevoNumero", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC ObtenerProximoNumeroRequisicion @NuevoNumero OUTPUT", nuevoNumeroParam);

                if (nuevoNumeroParam.Value == DBNull.Value || !int.TryParse(nuevoNumeroParam.Value.ToString(), out int nuevoNumero))
                {
                    return Json(new { success = false, errorMessage = "No se pudo obtener el número de requisición." });
                }

                // Crear la nueva transacción
                var transaccion = new Transaccion
                {
                    Ningresos = model.Ningresos,
                    CodigoAlmacen = model.CodigoAlmacen,
                    Nrequisicion = nuevoNumero,
                    FechaRequisicion = model.FechaRequisicion,
                    Operacion = model.Operacion,
                    FechaIngreso = model.FechaIngreso,
                    Observacion = model.Observacion
                };

                // Comienza la transacción de la base de datos
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                     
                    try
                    {
                        // Agregar la transacción principal
                        _context.Transaccions.Add(transaccion);

                        //Console.WriteLine("Artículos seleccionados:");
                        // Agregar los detalles de la transacción
                        foreach (var articulo in model.SelectedArticulos)
                        {

                            int codigo = articulo.Codigo;
                            int entrada = articulo.Entrada;
                            decimal precioUnitario = articulo.PrecioUnitario;

                            Console.WriteLine($"ID: {articulo.Codigo}, Entrada: {articulo.Entrada}, Precio: {articulo.PrecioUnitario}");
                            //Verificar si la Variable Key obtiene un Valor 

                            if (articulo.Codigo <= 0)
                            {
                                return Json(new { success = false, errorMessage = $"La entrada para el artículo {articulo.Codigo} debe ser mayor a cero." });
                            }


                            // Validar los valores de Entrada y Precio Unitario
                            if (articulo.Entrada <= 0)
                            {
                                return Json(new { success = false, errorMessage = $"La entrada para el artículo {articulo.Codigo} debe ser mayor a cero." });
                            }

                            if (articulo.Codigo <= 0)
                            {
                                return Json(new { success = false, errorMessage = $"La entrada para el artículo {articulo.Codigo} debe ser mayor a cero." });
                            }



                            // Crear el detalle de la transacción
                            var transaccionDetalle = new TransaccionDetalle
                            {
                                Ningresos = model.Ningresos,
                                CodigoAlmacen = model.CodigoAlmacen,
                                CodigoArticulo = codigo,
                                Operacion = model.Operacion,
                                FechaIngreso = model.FechaIngreso,
                                Entrada = entrada,
                                PrecioUnitario = precioUnitario
                            };
                            _context.TransaccionDetalles.Add(transaccionDetalle);
                               
                        }

                        // Asegúrate de que Operacion sea un string que puede ser "Ingreso" o "Salida"
                        // Se asume que model.Operacion se obtiene del ComboBox
                        if (model.Operacion != "Ingreso" && model.Operacion != "Salida")
                        {
                            return Json(new { success = false, errorMessage = "Operación no válida." });
                        }

                        // Asumiendo que Operacion es un string que puede ser "Ingreso" o "Salida"
                        foreach (var articulo in model.SelectedArticulos)
                        {
                            var articuloExistente = await _context.Articulos
                                .FirstOrDefaultAsync(a => a.IdentificadorArticulo == articulo.Codigo);

                            if (articuloExistente != null)
                            {
                                // Determinar si la operación es ingreso o salida
                                if (model.Operacion == "Ingreso") // Verifica si la operación es ingreso
                                {

                                    articuloExistente.PuntoReorden += articulo.Entrada; // Actualiza el campo de PuntoReorden para ingreso
                                    // Manejo del ingreso
                                    //if (articuloExistente.PuntoReorden + articulo.Entrada <= articuloExistente.PuntoMaximo)
                                    //{
                                       
                                    //}
                                    //else
                                    //{
                                    //    // Manejo del caso donde se supera el PuntoMaximo
                                    //    return Json(new { success = false, errorMessage = $"El PuntoReorden para el artículo {articulo.Codigo} excede el límite máximo permitido." });
                                    //}
                                }
                                else if (model.Operacion == "Salida") // Verifica si la operación es salida
                                {
                                    // Asegurarse de que Salida esté en el modelo
                                    if (articulo.Entrada <= 0)
                                    {
                                        return Json(new { success = false, errorMessage = $"La cantidad de salida debe ser mayor que cero para el artículo {articulo.Codigo}." });
                                    }

                                    // Manejo de la salida
                                    if (articuloExistente.PuntoReorden - articulo.Entrada >= 0) // Asegurarse de que no sea negativo
                                    {
                                        articuloExistente.PuntoReorden -= articulo.Entrada; // Actualiza el campo de PuntoReorden para salida
                                    }
                                    else
                                    {
                                        // Manejo del caso donde el PuntoReorden no puede ser negativo
                                        return Json(new { success = false, errorMessage = $"La cantidad solicitada supera las existencias disponibles para el artículo con código {articulo.Codigo}.." });
                                    }
                                }
                            }
                        }

                        // Guardar todos los cambios
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        HttpContext.Session.Remove("NumeroIngresos");



                        // Mensaje de éxito
                        return Json(new { success = true, message = "Transacción y detalles creados con éxito." });
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = ex.InnerException?.Message ?? ex.Message;
                        await transaction.RollbackAsync();
                        // Mensaje de error
                        return Json(new { success = false, errorMessage = "Error durante la creación de la transacción: " + errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Hubo un error inesperado. Detalles: " + ex.Message });
            }
        }



        //Método para el reporte de transacciones
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

            var transacciones = await _context.Transaccions
                .Include(t => t.TransaccionDetalles)
                .Where(t => t.CodigoAlmacen == userAlmacen.Identificador) // Filtrar por el almacén del usuario
                .Select(t => new TransaccionViewModel
                {
                    Ningresos = t.Ningresos,
                    CodigoAlmacen = t.CodigoAlmacen,
                    Nrequisicion = t.Nrequisicion,
                    FechaRequisicion = t.FechaRequisicion,
                    Operacion = t.Operacion,
                    FechaIngreso = t.FechaIngreso,
                    Observacion = t.Observacion,
                    TransaccionDetalles = t.TransaccionDetalles.Select(d => new TransaccionDetalleViewModel
                    {
                        CodigoArticulo = d.CodigoArticulo,
                        CodigoAlmacen = d.CodigoAlmacen,
                        Entrada = d.Entrada,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return View("Report", transacciones);
        }


    }
}
