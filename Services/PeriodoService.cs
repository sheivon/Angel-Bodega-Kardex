using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class PeriodoService
    {
        private readonly string _connectionString;

        public PeriodoService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS periodo (
  Id INT NOT NULL AUTO_INCREMENT,
  Anio INT DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_bin;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Periodo>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Anio, Eliminado FROM periodo WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Periodo>();
            while (await reader.ReadAsync())
            {
                results.Add(new Periodo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? null : reader.GetInt32("Anio"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Periodo?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Anio, Eliminado FROM periodo WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Periodo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? null : reader.GetInt32("Anio"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Periodo periodo)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO periodo (Anio) VALUES (@anio);";
            command.Parameters.AddWithValue("@anio", periodo.Anio.HasValue ? periodo.Anio.Value : (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Periodo periodo)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE periodo SET Anio = @anio WHERE Id = @id;";
            command.Parameters.AddWithValue("@anio", periodo.Anio.HasValue ? periodo.Anio.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@id", periodo.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE periodo SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
