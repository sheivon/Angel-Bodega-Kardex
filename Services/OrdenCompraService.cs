using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class OrdenCompraService
    {
        private readonly string _connectionString;

        public OrdenCompraService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySql")
                ?? throw new InvalidOperationException("MySql connection string not configured.");
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task EnsureTableExistsAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS orden_compra (
  Id INT NOT NULL AUTO_INCREMENT,
  Fecha_Ord_Compra DATE DEFAULT NULL,
  Numero VARCHAR(45) DEFAULT NULL,
  Id_Proyecto INT DEFAULT NULL,
  Tipo_Entrada VARCHAR(45) DEFAULT NULL,
  No_Proceso VARCHAR(60) DEFAULT NULL,
  Estado VARCHAR(45) DEFAULT NULL,
  Concepto VARCHAR(500) DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id),
  KEY Id_Proyecto_idx (Id_Proyecto)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<OrdenCompra>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Fecha_Ord_Compra, Numero, Id_Proyecto, Tipo_Entrada, No_Proceso, Estado, Concepto, Eliminado FROM orden_compra WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<OrdenCompra>();
            while (await reader.ReadAsync())
            {
                results.Add(new OrdenCompra
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FechaOrdCompra = reader.IsDBNull(reader.GetOrdinal("Fecha_Ord_Compra")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_Ord_Compra")),
                    Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetString(reader.GetOrdinal("Numero")),
                    IdProyecto = reader.IsDBNull(reader.GetOrdinal("Id_Proyecto")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Proyecto")),
                    TipoEntrada = reader.IsDBNull(reader.GetOrdinal("Tipo_Entrada")) ? null : reader.GetString(reader.GetOrdinal("Tipo_Entrada")),
                    NoProceso = reader.IsDBNull(reader.GetOrdinal("No_Proceso")) ? null : reader.GetString(reader.GetOrdinal("No_Proceso")),
                    Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                    Concepto = reader.IsDBNull(reader.GetOrdinal("Concepto")) ? null : reader.GetString(reader.GetOrdinal("Concepto")),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<OrdenCompra?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Fecha_Ord_Compra, Numero, Id_Proyecto, Tipo_Entrada, No_Proceso, Estado, Concepto, Eliminado FROM orden_compra WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new OrdenCompra
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FechaOrdCompra = reader.IsDBNull(reader.GetOrdinal("Fecha_Ord_Compra")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_Ord_Compra")),
                Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetString(reader.GetOrdinal("Numero")),
                IdProyecto = reader.IsDBNull(reader.GetOrdinal("Id_Proyecto")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Proyecto")),
                TipoEntrada = reader.IsDBNull(reader.GetOrdinal("Tipo_Entrada")) ? null : reader.GetString(reader.GetOrdinal("Tipo_Entrada")),
                NoProceso = reader.IsDBNull(reader.GetOrdinal("No_Proceso")) ? null : reader.GetString(reader.GetOrdinal("No_Proceso")),
                Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                Concepto = reader.IsDBNull(reader.GetOrdinal("Concepto")) ? null : reader.GetString(reader.GetOrdinal("Concepto")),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(OrdenCompra orden)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO orden_compra (Fecha_Ord_Compra, Numero, Id_Proyecto, Tipo_Entrada, No_Proceso, Estado, Concepto)
VALUES (@fecha, @numero, @idProyecto, @tipoEntrada, @noProceso, @estado, @concepto);";
            command.Parameters.AddWithValue("@fecha", orden.FechaOrdCompra.HasValue ? orden.FechaOrdCompra.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@numero", orden.Numero ?? string.Empty);
            command.Parameters.AddWithValue("@idProyecto", orden.IdProyecto.HasValue ? orden.IdProyecto.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@tipoEntrada", orden.TipoEntrada ?? string.Empty);
            command.Parameters.AddWithValue("@noProceso", orden.NoProceso ?? string.Empty);
            command.Parameters.AddWithValue("@estado", orden.Estado ?? string.Empty);
            command.Parameters.AddWithValue("@concepto", orden.Concepto ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(OrdenCompra orden)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE orden_compra SET Fecha_Ord_Compra = @fecha, Numero = @numero, Id_Proyecto = @idProyecto, Tipo_Entrada = @tipoEntrada, No_Proceso = @noProceso, Estado = @estado, Concepto = @concepto WHERE Id = @id;";
            command.Parameters.AddWithValue("@fecha", orden.FechaOrdCompra.HasValue ? orden.FechaOrdCompra.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@numero", orden.Numero ?? string.Empty);
            command.Parameters.AddWithValue("@idProyecto", orden.IdProyecto.HasValue ? orden.IdProyecto.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@tipoEntrada", orden.TipoEntrada ?? string.Empty);
            command.Parameters.AddWithValue("@noProceso", orden.NoProceso ?? string.Empty);
            command.Parameters.AddWithValue("@estado", orden.Estado ?? string.Empty);
            command.Parameters.AddWithValue("@concepto", orden.Concepto ?? string.Empty);
            command.Parameters.AddWithValue("@id", orden.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE orden_compra SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
