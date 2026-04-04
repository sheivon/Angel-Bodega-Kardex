using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class UnidadMedidaService
    {
        private readonly string _connectionString;

        public UnidadMedidaService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS unidad_medida (
  Id INT NOT NULL AUTO_INCREMENT,
  Nombre VARCHAR(45) DEFAULT NULL,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<UnidadMedida>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre FROM unidad_medida ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var result = new List<UnidadMedida>();
            while (await reader.ReadAsync())
            {
                result.Add(new UnidadMedida
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre")
                });
            }

            return result;
        }

        public async Task<UnidadMedida?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre FROM unidad_medida WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new UnidadMedida
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre")
            };
        }

        public async Task CreateAsync(UnidadMedida unidad)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO unidad_medida (Nombre) VALUES (@nombre);";
            command.Parameters.AddWithValue("@nombre", unidad.Nombre ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(UnidadMedida unidad)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE unidad_medida SET Nombre = @nombre WHERE Id = @id;";
            command.Parameters.AddWithValue("@nombre", unidad.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@id", unidad.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM unidad_medida WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
