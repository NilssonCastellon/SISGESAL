using System;
using System.Collections.Generic;

namespace SISGESAL.Models;

public partial class Transaccion
{
    public string Ningresos { get; set; } = null!;

    public int CodigoAlmacen { get; set; }

    public int Nrequisicion { get; set; }

    public DateTime FechaRequisicion { get; set; }

    public string Operacion { get; set; } = null!;

    public DateTime FechaIngreso { get; set; }

    public string? Observacion { get; set; }

    public virtual Almacene CodigoAlmacenNavigation { get; set; } = null!;

    public virtual ICollection<TransaccionDetalle> TransaccionDetalles { get; set; } = new List<TransaccionDetalle>();
}
