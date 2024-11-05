using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISGESAL.Models;
using SISGESAL.Models.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace SISGESAL.Controllers { 

[Authorize]

public class AlmacenesController : Controller
{
    private readonly SisgesalContext _context;

    public AlmacenesController(SisgesalContext context)
    {
        _context = context;
    }

    // GET: Almacenes
    public async Task<IActionResult> Index()
    {
        var almacenes = await _context.Almacenes
            .Select(a => new AlmaceneViewModel
            {
                Identificador = a.Identificador,
                Descripcion = a.Descripcion,
                Estado = a.Estado,
                FechaCreacion = a.FechaCreacion,
                UsuarioCreacion = a.UsuarioCreacion
            }).ToListAsync();

        return View(almacenes);
    }

    // Create
    public IActionResult Create()
    {
        return PartialView("_CreateModal", new AlmaceneViewModel());
    }

        // POST: Almacenes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlmaceneViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.Name; // Captura el nombre de usuario logueado

                    // Verificar si el usuario ya tiene un almacén
                    var existingAlmacen = await _context.Almacenes
                        .FirstOrDefaultAsync(a => a.UsuarioCreacion == userName);

                    if (existingAlmacen != null)
                    {
                        return Json(new { success = false, message = "Ya has creado un almacén." });
                    }

                    // Crear nuevo almacén
                    var almacene = new Almacene
                    {
                        Descripcion = viewModel.Descripcion,
                        Estado = true,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = userName // Asigna el nombre del usuario logueado
                    };

                    _context.Almacenes.Add(almacene);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Almacén creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error al crear el almacén: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Datos inválidos." });
        }


        // Edit
        public IActionResult Edit(int id)
    {
        var almacene = _context.Almacenes.Find(id);
        if (almacene == null)
        {
            return NotFound();
        }

        var viewModel = new AlmaceneViewModel
        {
            Identificador = almacene.Identificador,
            Descripcion = almacene.Descripcion,
        };

        return PartialView("_EditModal", viewModel);
    }

    // POST: Almacenes/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AlmaceneViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var almacene = await _context.Almacenes.FindAsync(viewModel.Identificador);
                if (almacene == null)
                {
                    return NotFound();
                }

                almacene.Descripcion = viewModel.Descripcion;

                _context.Update(almacene);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Configuración editada exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al editar la configuración: {ex.Message}" });
            }
        }

        return Json(new { success = false, message = "Datos inválidos." });
    }

    // POST: Almacenes/ChangeStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id)
    {
        try
        {
            var almacen = await _context.Almacenes.FindAsync(id);
            if (almacen == null)
            {
                return NotFound();
            }

            // Alternar el estado
            almacen.Estado = !almacen.Estado;

            _context.Update(almacen);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Estado del almacén cambiado exitosamente." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error al cambiar el estado: {ex.Message}" });
        }
    }


        // Reportes vista
        public async Task<IActionResult> Report()
        {
            var almacenes = await _context.Almacenes
                .Select(a => new AlmaceneViewModel
                {
                    Identificador = a.Identificador,
                    Descripcion = a.Descripcion,
                    Estado = a.Estado,
                    FechaCreacion = a.FechaCreacion,
                    UsuarioCreacion = a.UsuarioCreacion
                }).ToListAsync();

            // Devuelve la vista "Report" en la carpeta de vistas del controlador
            return View("Report", almacenes);
        }


    }
}