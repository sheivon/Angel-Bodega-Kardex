using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class CategoriaService
    {
        private readonly string _connectionString;

        public CategoriaService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS categoria (
  Id INT NOT NULL AUTO_INCREMENT,
  Nombre VARCHAR(200) CHARACTER SET utf8mb3 COLLATE utf8_general_ci DEFAULT NULL,
  Activo VARCHAR(3) CHARACTER SET utf8mb3 COLLATE utf8_general_ci DEFAULT NULL,
  Eliminado TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (Id)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Categoria>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Activo, Eliminado FROM categoria WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Categoria>();
            while (await reader.ReadAsync())
            {
                results.Add(new Categoria
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                    Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetString("Activo"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return results;
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Activo, Eliminado FROM categoria WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Categoria
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString("Nombre"),
                Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetString("Activo"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateAsync(Categoria categoria)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO categoria (Nombre, Activo) VALUES (@nombre, @activo);";
            command.Parameters.AddWithValue("@nombre", categoria.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@activo", categoria.Activo ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE categoria SET Nombre = @nombre, Activo = @activo WHERE Id = @id;";
            command.Parameters.AddWithValue("@nombre", categoria.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@activo", categoria.Activo ?? string.Empty);
            command.Parameters.AddWithValue("@id", categoria.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE categoria SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
