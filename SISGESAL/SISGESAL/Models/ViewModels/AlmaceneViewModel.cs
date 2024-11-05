
using System.ComponentModel.DataAnnotations;

namespace SISGESAL.Models.ViewModels;
public class AlmaceneViewModel
{

    public int Identificador { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(80, ErrorMessage = "La descripción no puede tener más de 80 caracteres.")]
    public string Descripcion
    {
        get => _descripcion;
        set => _descripcion = string.IsNullOrEmpty(value) ? null : value.ToUpper();
    }
    private string _descripcion = null!;

    public bool Estado { get; set; }

    public string? UsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

}
