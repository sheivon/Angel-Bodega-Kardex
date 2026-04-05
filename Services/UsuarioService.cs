using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class UsuarioService
    {
        private readonly string _connectionString;

        public UsuarioService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySql")
                ?? throw new InvalidOperationException("MySql connection string not configured.");
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task EnsureTableExistsAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var createCommand = connection.CreateCommand();
            createCommand.CommandText = @"CREATE TABLE IF NOT EXISTS `usuarios` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `Usuario` VARCHAR(45) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `Contraseña` VARCHAR(300) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `Cargo` VARCHAR(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `Lectura_Escritura` VARCHAR(2) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `Eliminado` TINYINT(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_bin;";
            await createCommand.ExecuteNonQueryAsync();

            await using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = @"ALTER TABLE `usuarios`
  ADD COLUMN IF NOT EXISTS `Eliminado` TINYINT(1) NOT NULL DEFAULT 0;";
            await alterCommand.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Usuario>> GetUsuariosAsync(bool includeDeleted = false)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = includeDeleted
                ? @"SELECT Id, Nombre, Usuario, Contraseña, Cargo, Lectura_Escritura AS LecturaEscritura, Eliminado FROM usuarios ORDER BY Id;"
                : @"SELECT Id, Nombre, Usuario, Contraseña, Cargo, Lectura_Escritura AS LecturaEscritura, Eliminado FROM usuarios WHERE Eliminado = 0 ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var result = new List<Usuario>();
            while (await reader.ReadAsync())
            {
                result.Add(new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Nombre = GetStringSafe(reader, "Nombre"),
                    UsuarioNombre = GetStringSafe(reader, "Usuario"),
                    Contraseña = GetStringSafe(reader, "Contraseña"),
                    Cargo = GetStringSafe(reader, "Cargo"),
                    LecturaEscritura = GetStringSafe(reader, "LecturaEscritura"),
                    Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
                });
            }

            return result;
        }

        public async Task<Usuario?> GetUsuarioAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Nombre, Usuario, Contraseña, Cargo, Lectura_Escritura AS LecturaEscritura, Eliminado
FROM usuarios
WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = GetStringSafe(reader, "Nombre"),
                UsuarioNombre = GetStringSafe(reader, "Usuario"),
                Contraseña = GetStringSafe(reader, "Contraseña"),
                Cargo = GetStringSafe(reader, "Cargo"),
                LecturaEscritura = GetStringSafe(reader, "LecturaEscritura"),
                Eliminado = reader.GetBoolean(reader.GetOrdinal("Eliminado"))
            };
        }

        public async Task CreateUsuarioAsync(Usuario usuario)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO usuarios
(Nombre, Usuario, `Contraseña`, Cargo, Lectura_Escritura)
VALUES (@nombre, @usuario, @contrasena, @cargo, @lecturaEscritura);";
            command.Parameters.AddWithValue("@nombre", usuario.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@usuario", usuario.UsuarioNombre ?? string.Empty);
            command.Parameters.AddWithValue("@contrasena", usuario.Contraseña ?? string.Empty);
            command.Parameters.AddWithValue("@cargo", usuario.Cargo ?? string.Empty);
            command.Parameters.AddWithValue("@lecturaEscritura", usuario.LecturaEscritura ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE usuarios SET
Nombre = @nombre,
Usuario = @usuario,
`Contraseña` = @contrasena,
Cargo = @cargo,
Lectura_Escritura = @lecturaEscritura
WHERE Id = @id;";
            command.Parameters.AddWithValue("@nombre", usuario.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@usuario", usuario.UsuarioNombre ?? string.Empty);
            command.Parameters.AddWithValue("@contrasena", usuario.Contraseña ?? string.Empty);
            command.Parameters.AddWithValue("@cargo", usuario.Cargo ?? string.Empty);
            command.Parameters.AddWithValue("@lecturaEscritura", usuario.LecturaEscritura ?? string.Empty);
            command.Parameters.AddWithValue("@id", usuario.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SoftDeleteUsuarioAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE usuarios SET Eliminado = 1 WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }

        private static string? GetStringSafe(MySqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}
