using System;
using System.Collections.Generic;

namespace SISGESAL.Models;

public partial class ConfiguraAlmacenArticulo
{
    public int IdentificadorConfig { get; set; }

    public int? IdentificadorAlmacen { get; set; }

    public int? IdentificadorArticulo { get; set; }

    public bool? Estado { get; set; }

    public virtual Almacene? IdentificadorAlmacenNavigation { get; set; }

    public virtual Articulo? IdentificadorArticuloNavigation { get; set; }
}
