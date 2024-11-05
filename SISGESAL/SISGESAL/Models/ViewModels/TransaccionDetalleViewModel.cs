using System.ComponentModel.DataAnnotations;

namespace SISGESAL.Models.ViewModels
{
    public class TransaccionDetalleViewModel
    {
        public int IdentificadorTrans { get; set; }

        [Required]
        public string Ningresos { get; set; } = null!;

        public int CodigoAlmacen { get; set; }

        public int? CodigoArticulo { get; set; } 

        public string Operacion { get; set; } = null!;

        public DateTime FechaIngreso { get; set; }

        public double? Entrada { get; set; } // Propiedad para entrada
        public decimal? PrecioUnitario { get; set; } // Propiedad para precio unitario

        // Utilizando un diccionario para mantener los artículos seleccionados y sus detalles
        public Dictionary<int, (int Entrada, decimal PrecioUnitario)> SelectedArticulos { get; set; } = new Dictionary<int, (int, decimal)>();


        public AlmaceneViewModel CodigoAlmacenNavigation { get; set; } = null!;
        public ArticuloViewModel CodigoArticuloNavigation { get; set; } = null!;
    }
}


