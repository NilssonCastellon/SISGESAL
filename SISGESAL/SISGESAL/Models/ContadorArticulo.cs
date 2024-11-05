using System;
using System.Collections.Generic;

namespace SISGESAL.Models;

public partial class ContadorArticulo
{
    public int Id { get; set; }

    public int CodigoAlmacen { get; set; }

    public int Contador { get; set; }

    public virtual Almacene CodigoAlmacenNavigation { get; set; } = null!;
}
