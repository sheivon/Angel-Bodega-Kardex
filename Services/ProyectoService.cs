using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class ProyectoService
    {
        private readonly string _connectionString;

        public ProyectoService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS proyecto (
  Id INT NOT NULL AUTO_INCREMENT,
  Nombre VARCHAR(400) DEFAULT NULL,
  Anio INT DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Proyecto>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Anio, Eliminado FROM proyecto WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Proyecto>();
            while (await reader.ReadAsync())
            {
                results.Add(new Proyecto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                    Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? null : reader.GetInt32("Anio"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Proyecto?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Anio, Eliminado FROM proyecto WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Proyecto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? null : reader.GetInt32("Anio"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Proyecto proyecto)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO proyecto (Nombre, Anio) VALUES (@nombre, @anio);";
            command.Parameters.AddWithValue("@nombre", proyecto.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@anio", proyecto.Anio.HasValue ? proyecto.Anio.Value : (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Proyecto proyecto)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE proyecto SET Nombre = @nombre, Anio = @anio WHERE Id = @id;";
            command.Parameters.AddWithValue("@nombre", proyecto.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@anio", proyecto.Anio.HasValue ? proyecto.Anio.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@id", proyecto.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE proyecto SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
