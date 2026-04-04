using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class AreaTrabajoService
    {
        private readonly string _connectionString;

        public AreaTrabajoService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS area_trabajo (
  Id INT NOT NULL AUTO_INCREMENT,
  Nombre VARCHAR(200) DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();

            command.CommandText = @"ALTER TABLE area_trabajo ADD COLUMN IF NOT EXISTS Eliminado TINYINT(1) NOT NULL DEFAULT 0;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<AreaTrabajo>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Eliminado FROM area_trabajo WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<AreaTrabajo>();
            while (await reader.ReadAsync())
            {
                results.Add(new AreaTrabajo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<AreaTrabajo?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Eliminado FROM area_trabajo WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new AreaTrabajo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(AreaTrabajo item)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO area_trabajo (Nombre) VALUES (@nombre);";
            command.Parameters.AddWithValue("@nombre", item.Nombre ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(AreaTrabajo item)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE area_trabajo SET Nombre = @nombre WHERE Id = @id;";
            command.Parameters.AddWithValue("@nombre", item.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@id", item.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE area_trabajo SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
