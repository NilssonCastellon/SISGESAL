using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SISGESAL.Models.ViewModels
{
    public class TransaccionViewModel
    {
        public IEnumerable<Almacene> Almacenes { get; set; }

        [Required]
        public string Ningresos { get; set; } = null!;

        [Required(ErrorMessage = "El campo Articulos es requerido.")]
        public int CodigoAlmacen { get; set; }

        [Required(ErrorMessage = "El campo Nrequisicion es requerido.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El campo Nrequisicion debe ser un número válido.")]
        public int Nrequisicion { get; set; }

        public DateTime FechaRequisicion { get; set; }

        public string Operacion { get; set; } = null!;

        public DateTime FechaIngreso { get; set; }

        [StringLength(250, ErrorMessage = "La observación no puede exceder los 250 caracteres.")]
        public string? Observacion { get; set; }

        public Almacene AlmacenSeleccionado { get; set; }

        public AlmaceneViewModel CodigoAlmacenNavigation { get; set; } = null!;
        public Almacene AlmacenPredeterminado { get; set; }

        // Utilizando un diccionario para mantener los artículos seleccionados y sus detalles
        //public Dictionary<int, (int Entrada, decimal PrecioUnitario)> SelectedArticulos { get; set; } = new Dictionary<int, (int, decimal)>();

        public List<ArticuloModel> SelectedArticulos { get; set; } = new List<ArticuloModel>();


        // Propiedades para TransaccionDetalle
        public ICollection<TransaccionDetalleViewModel> TransaccionDetalles { get; set; } = new List<TransaccionDetalleViewModel>();

        public List<Articulo> Articulos { get; set; }

        //public List<ArticuloDetalleViewModel> ArticulosDetalles { get; set; } = new List<ArticuloDetalleViewModel>();

        // Asegúrate de agregar esta propiedad
        public int TotalArticulos { get; set; }
    }

    public class ArticuloModel
    {
        public int Codigo { get; set; }
        public int Entrada { get; set; }
        public decimal PrecioUnitario { get; set; }

    }

}


