using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class RetiraService
    {
        private readonly string _connectionString;

        public RetiraService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS retira (
  Id INT NOT NULL AUTO_INCREMENT,
  Cedula VARCHAR(45) DEFAULT NULL,
  Nombres_Apellidos VARCHAR(45) DEFAULT NULL,
  Cargo VARCHAR(45) DEFAULT NULL,
  AyudaSocial VARCHAR(2) DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Retira>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Cedula, Nombres_Apellidos, Cargo, AyudaSocial, Eliminado FROM retira WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Retira>();
            while (await reader.ReadAsync())
            {
                results.Add(new Retira
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Cedula = reader.IsDBNull(reader.GetOrdinal("Cedula")) ? null : reader.GetString("Cedula"),
                    NombresApellidos = reader.IsDBNull(reader.GetOrdinal("Nombres_Apellidos")) ? null : reader.GetString("Nombres_Apellidos"),
                    Cargo = reader.IsDBNull(reader.GetOrdinal("Cargo")) ? null : reader.GetString("Cargo"),
                    AyudaSocial = reader.IsDBNull(reader.GetOrdinal("AyudaSocial")) ? null : reader.GetString("AyudaSocial"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Retira?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Cedula, Nombres_Apellidos, Cargo, AyudaSocial, Eliminado FROM retira WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Retira
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Cedula = reader.IsDBNull(reader.GetOrdinal("Cedula")) ? null : reader.GetString("Cedula"),
                NombresApellidos = reader.IsDBNull(reader.GetOrdinal("Nombres_Apellidos")) ? null : reader.GetString("Nombres_Apellidos"),
                Cargo = reader.IsDBNull(reader.GetOrdinal("Cargo")) ? null : reader.GetString("Cargo"),
                AyudaSocial = reader.IsDBNull(reader.GetOrdinal("AyudaSocial")) ? null : reader.GetString("AyudaSocial"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Retira item)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO retira (Cedula, Nombres_Apellidos, Cargo, AyudaSocial) VALUES (@cedula, @nombres, @cargo, @ayudasocial);";
            command.Parameters.AddWithValue("@cedula", item.Cedula ?? string.Empty);
            command.Parameters.AddWithValue("@nombres", item.NombresApellidos ?? string.Empty);
            command.Parameters.AddWithValue("@cargo", item.Cargo ?? string.Empty);
            command.Parameters.AddWithValue("@ayudasocial", item.AyudaSocial ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Retira item)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE retira SET Cedula = @cedula, Nombres_Apellidos = @nombres, Cargo = @cargo, AyudaSocial = @ayudasocial WHERE Id = @id;";
            command.Parameters.AddWithValue("@cedula", item.Cedula ?? string.Empty);
            command.Parameters.AddWithValue("@nombres", item.NombresApellidos ?? string.Empty);
            command.Parameters.AddWithValue("@cargo", item.Cargo ?? string.Empty);
            command.Parameters.AddWithValue("@ayudasocial", item.AyudaSocial ?? string.Empty);
            command.Parameters.AddWithValue("@id", item.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE retira SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
