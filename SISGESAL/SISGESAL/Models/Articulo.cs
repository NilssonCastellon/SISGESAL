using System;
using System.Collections.Generic;

namespace SISGESAL.Models;

public partial class Articulo
{
    public int IdentificadorArticulo { get; set; }

    public int CodigoArticulo { get; set; }

    public string? DescripcionArticulo { get; set; }

    public string? TipoArticulo { get; set; }

    public decimal? PuntoMinimo { get; set; }

    public decimal? PuntoMaximo { get; set; }

    public decimal? PuntoReorden { get; set; }

    public string? UsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool Estado { get; set; }

    public int? CodigoAlmacen { get; set; }

   

    public virtual Almacene? CodigoAlmacenNavigation { get; set; }

    public virtual ICollection<ConfiguraAlmacenArticulo> ConfiguraAlmacenArticulos { get; set; } = new List<ConfiguraAlmacenArticulo>();

    public virtual ICollection<TransaccionDetalle> TransaccionDetalles { get; set; } = new List<TransaccionDetalle>();
}
