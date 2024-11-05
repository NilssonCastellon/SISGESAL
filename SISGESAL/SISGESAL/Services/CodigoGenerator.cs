using System.Data;
using Microsoft.Data.SqlClient;

public class CodigoGenerator
{
    private readonly string _connectionString;

    public CodigoGenerator(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string GenerarCodigo(int codigoAlmacen)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand("GenerarCodigo", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Parámetro de entrada, debe coincidir con el nombre esperado por el procedimiento almacenado
                command.Parameters.AddWithValue("@CodigoAlmacen", codigoAlmacen);

                // Parámetro de salida
                SqlParameter outputParam = new SqlParameter("@NuevoCodigo", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                // Ejecutar el procedimiento
                command.ExecuteNonQuery();

                // Obtener el código generado
                return (string)outputParam.Value;
            }
        }
    }
}
