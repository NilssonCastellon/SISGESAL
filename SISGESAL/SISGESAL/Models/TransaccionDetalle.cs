using System;
using System.Collections.Generic;

namespace SISGESAL.Models;

public partial class TransaccionDetalle
{
    public int IdentificadorTrans { get; set; }

    public string Ningresos { get; set; } = null!;

    public int CodigoAlmacen { get; set; }

    public int? CodigoArticulo { get; set; }

    public string Operacion { get; set; } = null!;

    public DateTime FechaIngreso { get; set; }

    public double? Entrada { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public virtual Almacene CodigoAlmacenNavigation { get; set; } = null!;

    public virtual Articulo? CodigoArticuloNavigation { get; set; }

    public virtual Transaccion NingresosNavigation { get; set; } = null!;
}
