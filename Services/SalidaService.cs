using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class SalidaService
    {
        private readonly string _connectionString;

        public SalidaService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS salida (
  Id INT NOT NULL AUTO_INCREMENT,
  Numero_S INT DEFAULT NULL,
  Fecha DATE DEFAULT NULL,
  Ayuda VARCHAR(2) DEFAULT NULL,
  Id_Autoriza INT DEFAULT NULL,
  Id_Retira INT DEFAULT NULL,
  Estado VARCHAR(45) DEFAULT NULL,
  Nombre_Registra VARCHAR(100) DEFAULT NULL,
  Concepto VARCHAR(500) DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id),
  KEY Id_Autoriza_idx (Id_Autoriza),
  KEY Id_Retira_idx (Id_Retira)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Salida>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Numero_S, Fecha, Ayuda, Id_Autoriza, Id_Retira, Estado, Nombre_Registra, Concepto, Eliminado
FROM salida
WHERE Eliminado = 0
ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Salida>();
            while (await reader.ReadAsync())
            {
                results.Add(new Salida
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    NumeroS = reader.IsDBNull(reader.GetOrdinal("Numero_S")) ? null : reader.GetInt32("Numero_S"),
                    Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime("Fecha"),
                    Ayuda = reader.IsDBNull(reader.GetOrdinal("Ayuda")) ? null : reader.GetString("Ayuda"),
                    IdAutoriza = reader.IsDBNull(reader.GetOrdinal("Id_Autoriza")) ? null : reader.GetInt32("Id_Autoriza"),
                    IdRetira = reader.IsDBNull(reader.GetOrdinal("Id_Retira")) ? null : reader.GetInt32("Id_Retira"),
                    Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString("Estado"),
                    NombreRegistra = reader.IsDBNull(reader.GetOrdinal("Nombre_Registra")) ? null : reader.GetString("Nombre_Registra"),
                    Concepto = reader.IsDBNull(reader.GetOrdinal("Concepto")) ? null : reader.GetString("Concepto"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Salida?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Numero_S, Fecha, Ayuda, Id_Autoriza, Id_Retira, Estado, Nombre_Registra, Concepto, Eliminado
FROM salida
WHERE Id = @id
LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Salida
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                NumeroS = reader.IsDBNull(reader.GetOrdinal("Numero_S")) ? null : reader.GetInt32("Numero_S"),
                Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime("Fecha"),
                Ayuda = reader.IsDBNull(reader.GetOrdinal("Ayuda")) ? null : reader.GetString("Ayuda"),
                IdAutoriza = reader.IsDBNull(reader.GetOrdinal("Id_Autoriza")) ? null : reader.GetInt32("Id_Autoriza"),
                IdRetira = reader.IsDBNull(reader.GetOrdinal("Id_Retira")) ? null : reader.GetInt32("Id_Retira"),
                Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString("Estado"),
                NombreRegistra = reader.IsDBNull(reader.GetOrdinal("Nombre_Registra")) ? null : reader.GetString("Nombre_Registra"),
                Concepto = reader.IsDBNull(reader.GetOrdinal("Concepto")) ? null : reader.GetString("Concepto"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Salida salida)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO salida (Numero_S, Fecha, Ayuda, Id_Autoriza, Id_Retira, Estado, Nombre_Registra, Concepto)
VALUES (@numero, @fecha, @ayuda, @idAutoriza, @idRetira, @estado, @nombreRegistra, @concepto);";
            command.Parameters.AddWithValue("@numero", salida.NumeroS.HasValue ? salida.NumeroS.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fecha", salida.Fecha.HasValue ? salida.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@ayuda", salida.Ayuda ?? string.Empty);
            command.Parameters.AddWithValue("@idAutoriza", salida.IdAutoriza.HasValue ? salida.IdAutoriza.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idRetira", salida.IdRetira.HasValue ? salida.IdRetira.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@estado", salida.Estado ?? string.Empty);
            command.Parameters.AddWithValue("@nombreRegistra", salida.NombreRegistra ?? string.Empty);
            command.Parameters.AddWithValue("@concepto", salida.Concepto ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Salida salida)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE salida SET Numero_S = @numero, Fecha = @fecha, Ayuda = @ayuda, Id_Autoriza = @idAutoriza, Id_Retira = @idRetira, Estado = @estado, Nombre_Registra = @nombreRegistra, Concepto = @concepto WHERE Id = @id;";
            command.Parameters.AddWithValue("@numero", salida.NumeroS.HasValue ? salida.NumeroS.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fecha", salida.Fecha.HasValue ? salida.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@ayuda", salida.Ayuda ?? string.Empty);
            command.Parameters.AddWithValue("@idAutoriza", salida.IdAutoriza.HasValue ? salida.IdAutoriza.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idRetira", salida.IdRetira.HasValue ? salida.IdRetira.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@estado", salida.Estado ?? string.Empty);
            command.Parameters.AddWithValue("@nombreRegistra", salida.NombreRegistra ?? string.Empty);
            command.Parameters.AddWithValue("@concepto", salida.Concepto ?? string.Empty);
            command.Parameters.AddWithValue("@id", salida.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE salida SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
