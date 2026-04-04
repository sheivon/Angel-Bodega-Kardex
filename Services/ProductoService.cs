using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class ProductoService
    {
        private readonly string _connectionString;

        public ProductoService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS productos (
                  Id INT NOT NULL AUTO_INCREMENT,
                  Codigo VARCHAR(45) CHARACTER SET utf8mb3 COLLATE utf8_general_ci DEFAULT NULL,
                  Nombre VARCHAR(201) CHARACTER SET utf8mb3 COLLATE utf8_general_ci DEFAULT NULL,
                  Id_Categoria INT DEFAULT NULL,
                  Id_U_M INT DEFAULT NULL,
                  PRIMARY KEY (Id),
                  KEY Id_Categoria_idx (Id_Categoria),
                  KEY Id_U_M_idx (Id_U_M),
                  CONSTRAINT Id_Categoria FOREIGN KEY (Id_Categoria) REFERENCES categoria (Id),
                  CONSTRAINT Id_U_M FOREIGN KEY (Id_U_M) REFERENCES unidad_medida (Id)
                ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1 COLLATE=latin1_bin;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Producto>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Codigo, Nombre, Id_Categoria, Id_U_M FROM productos ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Producto>();
            while (await reader.ReadAsync())
            {
                results.Add(new Producto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Codigo = reader.IsDBNull(reader.GetOrdinal("Codigo")) ? null : reader.GetString(reader.GetOrdinal("Codigo")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                    IdCategoria = reader.IsDBNull(reader.GetOrdinal("Id_Categoria")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Categoria")),
                    IdUM = reader.IsDBNull(reader.GetOrdinal("Id_U_M")) ? null : reader.GetInt32(reader.GetOrdinal("Id_U_M"))
                });
            }

            return results;
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Codigo, Nombre, Id_Categoria, Id_U_M FROM productos WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Producto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Codigo = reader.IsDBNull(reader.GetOrdinal("Codigo")) ? null : reader.GetString(reader.GetOrdinal("Codigo")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                IdCategoria = reader.IsDBNull(reader.GetOrdinal("Id_Categoria")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Categoria")),
                IdUM = reader.IsDBNull(reader.GetOrdinal("Id_U_M")) ? null : reader.GetInt32(reader.GetOrdinal("Id_U_M"))
            };
        }

        public async Task CreateAsync(Producto producto)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO productos (Codigo, Nombre, Id_Categoria, Id_U_M) VALUES (@codigo, @nombre, @idCategoria, @idUM);";
            command.Parameters.AddWithValue("@codigo", producto.Codigo ?? string.Empty);
            command.Parameters.AddWithValue("@nombre", producto.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@idCategoria", producto.IdCategoria.HasValue ? producto.IdCategoria.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idUM", producto.IdUM.HasValue ? producto.IdUM.Value : (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Producto producto)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE productos SET Codigo = @codigo, Nombre = @nombre, Id_Categoria = @idCategoria, Id_U_M = @idUM WHERE Id = @id;";
            command.Parameters.AddWithValue("@codigo", producto.Codigo ?? string.Empty);
            command.Parameters.AddWithValue("@nombre", producto.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@idCategoria", producto.IdCategoria.HasValue ? producto.IdCategoria.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idUM", producto.IdUM.HasValue ? producto.IdUM.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@id", producto.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM productos WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
