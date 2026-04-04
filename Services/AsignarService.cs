using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class AsignarService
    {
        private readonly string _connectionString;

        public AsignarService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS asignar (
  Id INT NOT NULL AUTO_INCREMENT,
  Id_Activo INT DEFAULT NULL,
  Fecha DATE DEFAULT NULL,
  Id_Retira INT DEFAULT NULL,
  Nombres_Apellidos VARCHAR(150) DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id),
  KEY Id_Activo_idx (Id_Activo),
  CONSTRAINT Id_Activo FOREIGN KEY (Id_Activo) REFERENCES activo (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Asignar>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Id_Activo, Fecha, Id_Retira, Nombres_Apellidos, Eliminado FROM asignar WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Asignar>();
            while (await reader.ReadAsync())
            {
                results.Add(new Asignar
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    IdActivo = reader.IsDBNull(reader.GetOrdinal("Id_Activo")) ? null : reader.GetInt32("Id_Activo"),
                    Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime("Fecha"),
                    IdRetira = reader.IsDBNull(reader.GetOrdinal("Id_Retira")) ? null : reader.GetInt32("Id_Retira"),
                    NombresApellidos = reader.IsDBNull(reader.GetOrdinal("Nombres_Apellidos")) ? null : reader.GetString("Nombres_Apellidos"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Asignar?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Id_Activo, Fecha, Id_Retira, Nombres_Apellidos, Eliminado FROM asignar WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Asignar
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                IdActivo = reader.IsDBNull(reader.GetOrdinal("Id_Activo")) ? null : reader.GetInt32("Id_Activo"),
                Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime("Fecha"),
                IdRetira = reader.IsDBNull(reader.GetOrdinal("Id_Retira")) ? null : reader.GetInt32("Id_Retira"),
                NombresApellidos = reader.IsDBNull(reader.GetOrdinal("Nombres_Apellidos")) ? null : reader.GetString("Nombres_Apellidos"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Asignar item)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO asignar (Id_Activo, Fecha, Id_Retira, Nombres_Apellidos) VALUES (@idActivo, @fecha, @idRetira, @nombres);";
            command.Parameters.AddWithValue("@idActivo", item.IdActivo.HasValue ? item.IdActivo.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fecha", item.Fecha.HasValue ? item.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idRetira", item.IdRetira.HasValue ? item.IdRetira.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@nombres", item.NombresApellidos ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Asignar item)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE asignar SET Id_Activo = @idActivo, Fecha = @fecha, Id_Retira = @idRetira, Nombres_Apellidos = @nombres WHERE Id = @id;";
            command.Parameters.AddWithValue("@idActivo", item.IdActivo.HasValue ? item.IdActivo.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fecha", item.Fecha.HasValue ? item.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idRetira", item.IdRetira.HasValue ? item.IdRetira.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@nombres", item.NombresApellidos ?? string.Empty);
            command.Parameters.AddWithValue("@id", item.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE asignar SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
