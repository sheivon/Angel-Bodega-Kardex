using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class ProveedorService
    {
        private readonly string _connectionString;

        public ProveedorService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS proveedor (
  Id INT NOT NULL AUTO_INCREMENT,
  Nombre VARCHAR(45) DEFAULT NULL,
  Telefono INT DEFAULT NULL,
  Direccion VARCHAR(60) DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Proveedor>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Telefono, Direccion, Eliminado FROM proveedor WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Proveedor>();
            while (await reader.ReadAsync())
            {
                results.Add(new Proveedor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetInt32("Telefono"),
                    Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Proveedor?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Telefono, Direccion, Eliminado FROM proveedor WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Proveedor
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetInt32("Telefono"),
                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Proveedor proveedor)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO proveedor (Nombre, Telefono, Direccion) VALUES (@nombre, @telefono, @direccion);";
            command.Parameters.AddWithValue("@nombre", proveedor.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@telefono", proveedor.Telefono.HasValue ? proveedor.Telefono.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@direccion", proveedor.Direccion ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Proveedor proveedor)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE proveedor SET Nombre = @nombre, Telefono = @telefono, Direccion = @direccion WHERE Id = @id;";
            command.Parameters.AddWithValue("@nombre", proveedor.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@telefono", proveedor.Telefono.HasValue ? proveedor.Telefono.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@direccion", proveedor.Direccion ?? string.Empty);
            command.Parameters.AddWithValue("@id", proveedor.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE proveedor SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
