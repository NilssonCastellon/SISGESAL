using System;
using System.Collections.Generic;

namespace SISGESAL.Models;

public partial class Almacene
{
    public int Identificador { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Estado { get; set; }

    public string? UsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    public virtual ICollection<ConfiguraAlmacenArticulo> ConfiguraAlmacenArticulos { get; set; } = new List<ConfiguraAlmacenArticulo>();

    public virtual ICollection<ContadorArticulo> ContadorArticulos { get; set; } = new List<ContadorArticulo>();

    public virtual ICollection<TransaccionDetalle> TransaccionDetalles { get; set; } = new List<TransaccionDetalle>();

    public virtual ICollection<Transaccion> Transaccions { get; set; } = new List<Transaccion>();
}
