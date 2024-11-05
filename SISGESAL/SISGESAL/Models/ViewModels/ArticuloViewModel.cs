using System.ComponentModel.DataAnnotations;

namespace SISGESAL.Models.ViewModels
{
    public class ArticuloViewModel
    {
        public int IdentificadorArticulo { get; set; }

        public int CodigoArticulo { get; set; }

        [StringLength(80, ErrorMessage = "La descripción debe tener un máximo de 80 caracteres.")]
        public string? DescripcionArticulo
        {
            get => _descripcionArticulo;
            set => _descripcionArticulo = string.IsNullOrEmpty(value) ? null : value.ToUpper();
        }
        private string? _descripcionArticulo;

        [StringLength(80, ErrorMessage = "El tipo de artículo debe tener un máximo de 80 caracteres.")]
        public string? TipoArticulo
        {
            get => _tipoArticulo;
            set => _tipoArticulo = string.IsNullOrEmpty(value) ? null : value.ToUpper();
        }
        private string? _tipoArticulo;

        public decimal? PuntoMinimo { get; set; }
        public decimal? PuntoMaximo { get; set; }
        public decimal? PuntoReorden { get; set; }
    

        public string? UsuarioCreacion { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public bool Estado { get; set; }

        public int CodigoAlmacen { get; set; }

       
    }
}
////Transacción detalles 
//public int Entrada { get; set; }            // Cantidad de entrada
//public decimal PrecioUnitario { get; set; } // Precio unitario