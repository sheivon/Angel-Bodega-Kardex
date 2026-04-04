using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class EntradaService
    {
        private readonly string _connectionString;

        public EntradaService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS entrada (
  Id INT NOT NULL AUTO_INCREMENT,
  Numero_E INT DEFAULT NULL,
  Fecha DATE DEFAULT NULL,
  Id_Ord_Compra INT DEFAULT NULL,
  Id_Proveedor INT DEFAULT NULL,
  Fecha_Factura DATE DEFAULT NULL,
  No_Factura VARCHAR(150) DEFAULT NULL,
  Estado VARCHAR(45) DEFAULT NULL,
  No_CK VARCHAR(45) DEFAULT NULL,
  Nombre_Autoriza VARCHAR(150) DEFAULT NULL,
  Nombre_Registra VARCHAR(150) DEFAULT NULL,
  PRIMARY KEY (Id),
  KEY Id_Ord_Compra_idx (Id_Ord_Compra),
  KEY Id_Proveedor_idx (Id_Proveedor),
  CONSTRAINT Id_Ord_Compra FOREIGN KEY (Id_Ord_Compra) REFERENCES orden_compra (Id),
  CONSTRAINT Id_Proveedor FOREIGN KEY (Id_Proveedor) REFERENCES proveedor (Id)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Entrada>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Numero_E, Fecha, Id_Ord_Compra, Id_Proveedor, Fecha_Factura, No_Factura, Estado, No_CK, Nombre_Autoriza, Nombre_Registra FROM entrada ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Entrada>();
            while (await reader.ReadAsync())
            {
                results.Add(new Entrada
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    NumeroE = reader.IsDBNull(reader.GetOrdinal("Numero_E")) ? null : reader.GetInt32(reader.GetOrdinal("Numero_E")),
                    Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    IdOrdCompra = reader.IsDBNull(reader.GetOrdinal("Id_Ord_Compra")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Ord_Compra")),
                    IdProveedor = reader.IsDBNull(reader.GetOrdinal("Id_Proveedor")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Proveedor")),
                    FechaFactura = reader.IsDBNull(reader.GetOrdinal("Fecha_Factura")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                    NoFactura = reader.IsDBNull(reader.GetOrdinal("No_Factura")) ? null : reader.GetString(reader.GetOrdinal("No_Factura")),
                    Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                    NoCK = reader.IsDBNull(reader.GetOrdinal("No_CK")) ? null : reader.GetString(reader.GetOrdinal("No_CK")),
                    NombreAutoriza = reader.IsDBNull(reader.GetOrdinal("Nombre_Autoriza")) ? null : reader.GetString(reader.GetOrdinal("Nombre_Autoriza")),
                    NombreRegistra = reader.IsDBNull(reader.GetOrdinal("Nombre_Registra")) ? null : reader.GetString(reader.GetOrdinal("Nombre_Registra"))
                });
            }

            return results;
        }

        public async Task<Entrada?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Numero_E, Fecha, Id_Ord_Compra, Id_Proveedor, Fecha_Factura, No_Factura, Estado, No_CK, Nombre_Autoriza, Nombre_Registra FROM entrada WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Entrada
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                NumeroE = reader.IsDBNull(reader.GetOrdinal("Numero_E")) ? null : reader.GetInt32(reader.GetOrdinal("Numero_E")),
                Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha")),
                IdOrdCompra = reader.IsDBNull(reader.GetOrdinal("Id_Ord_Compra")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Ord_Compra")),
                IdProveedor = reader.IsDBNull(reader.GetOrdinal("Id_Proveedor")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Proveedor")),
                FechaFactura = reader.IsDBNull(reader.GetOrdinal("Fecha_Factura")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                NoFactura = reader.IsDBNull(reader.GetOrdinal("No_Factura")) ? null : reader.GetString(reader.GetOrdinal("No_Factura")),
                Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                NoCK = reader.IsDBNull(reader.GetOrdinal("No_CK")) ? null : reader.GetString(reader.GetOrdinal("No_CK")),
                NombreAutoriza = reader.IsDBNull(reader.GetOrdinal("Nombre_Autoriza")) ? null : reader.GetString(reader.GetOrdinal("Nombre_Autoriza")),
                NombreRegistra = reader.IsDBNull(reader.GetOrdinal("Nombre_Registra")) ? null : reader.GetString(reader.GetOrdinal("Nombre_Registra"))
            };
        }

        public async Task CreateAsync(Entrada entrada)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO entrada (Numero_E, Fecha, Id_Ord_Compra, Id_Proveedor, Fecha_Factura, No_Factura, Estado, No_CK, Nombre_Autoriza, Nombre_Registra)
VALUES (@numeroE, @fecha, @idOrdCompra, @idProveedor, @fechaFactura, @noFactura, @estado, @noCK, @nombreAutoriza, @nombreRegistra);";
            command.Parameters.AddWithValue("@numeroE", entrada.NumeroE.HasValue ? entrada.NumeroE.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fecha", entrada.Fecha.HasValue ? entrada.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idOrdCompra", entrada.IdOrdCompra.HasValue ? entrada.IdOrdCompra.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProveedor", entrada.IdProveedor.HasValue ? entrada.IdProveedor.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fechaFactura", entrada.FechaFactura.HasValue ? entrada.FechaFactura.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@noFactura", entrada.NoFactura ?? string.Empty);
            command.Parameters.AddWithValue("@estado", entrada.Estado ?? string.Empty);
            command.Parameters.AddWithValue("@noCK", entrada.NoCK ?? string.Empty);
            command.Parameters.AddWithValue("@nombreAutoriza", entrada.NombreAutoriza ?? string.Empty);
            command.Parameters.AddWithValue("@nombreRegistra", entrada.NombreRegistra ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Entrada entrada)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE entrada SET Numero_E = @numeroE, Fecha = @fecha, Id_Ord_Compra = @idOrdCompra, Id_Proveedor = @idProveedor, Fecha_Factura = @fechaFactura, No_Factura = @noFactura, Estado = @estado, No_CK = @noCK, Nombre_Autoriza = @nombreAutoriza, Nombre_Registra = @nombreRegistra WHERE Id = @id;";
            command.Parameters.AddWithValue("@numeroE", entrada.NumeroE.HasValue ? entrada.NumeroE.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fecha", entrada.Fecha.HasValue ? entrada.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idOrdCompra", entrada.IdOrdCompra.HasValue ? entrada.IdOrdCompra.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProveedor", entrada.IdProveedor.HasValue ? entrada.IdProveedor.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fechaFactura", entrada.FechaFactura.HasValue ? entrada.FechaFactura.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@noFactura", entrada.NoFactura ?? string.Empty);
            command.Parameters.AddWithValue("@estado", entrada.Estado ?? string.Empty);
            command.Parameters.AddWithValue("@noCK", entrada.NoCK ?? string.Empty);
            command.Parameters.AddWithValue("@nombreAutoriza", entrada.NombreAutoriza ?? string.Empty);
            command.Parameters.AddWithValue("@nombreRegistra", entrada.NombreRegistra ?? string.Empty);
            command.Parameters.AddWithValue("@id", entrada.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM entrada WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
