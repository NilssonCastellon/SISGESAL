using System.ComponentModel.DataAnnotations;

namespace SISGESAL.Models.ViewModels
{
    public class ConfiguraAlmacenArticuloViewModel
    {
        public int IdentificadorConfig { get; set; }

        [Required(ErrorMessage = "El almacén es obligatorio.")]
        public int? IdentificadorAlmacen { get; set; }

 
        [Required(ErrorMessage = "El artículo es obligatorio.")]
        public int? IdentificadorArticulo { get; set; }

        [Display(Name = "Estado")]
        public bool? Estado { get; set; }
        public int CodigoAlmacen { get; set; } // Asegúrate de tener esta propiedad
        public string AlmacenDescripcion { get; set; } // Propiedad agregada
        public string ArticuloDescripcion { get; set; } // Propiedad agregada
    }
}
