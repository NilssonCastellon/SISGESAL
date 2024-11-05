using System.Data;
using System.Data;
using Microsoft.Data.SqlClient;
public class RequisicionService
{
    private readonly IConfiguration _configuration;

    public RequisicionService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int ObtenerProximoNumeroRequisicion()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("SISGESALDbContextConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("ObtenerProximoNumeroRequisicion", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var outputParam = new SqlParameter("@NuevoNumero", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputParam.Value;
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo del error
            throw new Exception("Error al obtener el próximo número de requisición.", ex);
        }
    }

}
