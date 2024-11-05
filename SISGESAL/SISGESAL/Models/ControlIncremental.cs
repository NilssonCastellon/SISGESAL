using System;
using System.Collections.Generic;

namespace SISGESAL.Models;

public partial class ControlIncremental
{
    public int Año { get; set; }

    public int? Numero { get; set; }

    public string CodigoAlmacen { get; set; } = null!;
}
