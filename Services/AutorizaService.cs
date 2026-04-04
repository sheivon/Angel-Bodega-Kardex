using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class AutorizaService
    {
        private readonly string _connectionString;

        public AutorizaService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS autoriza (
  Id INT NOT NULL AUTO_INCREMENT,
  Cargo VARCHAR(100) DEFAULT NULL,
  Nombres_Apellidos VARCHAR(100) DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Autoriza>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Cargo, Nombres_Apellidos, Eliminado
FROM autoriza
WHERE Eliminado = 0
ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Autoriza>();
            while (await reader.ReadAsync())
            {
                results.Add(new Autoriza
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Cargo = reader.IsDBNull(reader.GetOrdinal("Cargo")) ? null : reader.GetString("Cargo"),
                    NombresApellidos = reader.IsDBNull(reader.GetOrdinal("Nombres_Apellidos")) ? null : reader.GetString("Nombres_Apellidos"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Autoriza?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Cargo, Nombres_Apellidos, Eliminado
FROM autoriza
WHERE Id = @id
LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Autoriza
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Cargo = reader.IsDBNull(reader.GetOrdinal("Cargo")) ? null : reader.GetString("Cargo"),
                NombresApellidos = reader.IsDBNull(reader.GetOrdinal("Nombres_Apellidos")) ? null : reader.GetString("Nombres_Apellidos"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Autoriza autoriza)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO autoriza (Cargo, Nombres_Apellidos) VALUES (@cargo, @nombres);";
            command.Parameters.AddWithValue("@cargo", autoriza.Cargo ?? string.Empty);
            command.Parameters.AddWithValue("@nombres", autoriza.NombresApellidos ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Autoriza autoriza)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE autoriza SET Cargo = @cargo, Nombres_Apellidos = @nombres WHERE Id = @id;";
            command.Parameters.AddWithValue("@cargo", autoriza.Cargo ?? string.Empty);
            command.Parameters.AddWithValue("@nombres", autoriza.NombresApellidos ?? string.Empty);
            command.Parameters.AddWithValue("@id", autoriza.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE autoriza SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
