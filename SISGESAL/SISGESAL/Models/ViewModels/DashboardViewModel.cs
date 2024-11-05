namespace SISGESAL.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalArticulos { get; set; }
        public List<Articulo> ArticulosBajoStock { get; set; }

        public string NombreAlmacen { get; set; } // Nombre del almacén
        public int TotalTransacciones { get; set; } // Número de transacciones realizadas
    }
}
