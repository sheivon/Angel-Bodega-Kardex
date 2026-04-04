using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class ActivoService
    {
        private readonly string _connectionString;

        public ActivoService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS activo (
  Id INT NOT NULL AUTO_INCREMENT,
  Fecha DATE DEFAULT NULL,
  Id_Categoria INT DEFAULT NULL,
  Id_Productos INT DEFAULT NULL,
  Codigo_Contable VARCHAR(45) DEFAULT NULL,
  No_Serie VARCHAR(45) DEFAULT NULL,
  Modelo VARCHAR(45) DEFAULT NULL,
  Marca VARCHAR(45) DEFAULT NULL,
  Nombre VARCHAR(150) DEFAULT NULL,
  Precio DECIMAL(15,2) DEFAULT NULL,
  IVA DECIMAL(15,2) DEFAULT NULL,
  C_Total DECIMAL(15,2) DEFAULT NULL,
  Id_Movimientos INT DEFAULT NULL,
  Id_Salida INT DEFAULT NULL,
  Id_Entrada INT DEFAULT NULL,
  No_Factura INT DEFAULT NULL,
  Fecha_Factura DATE DEFAULT NULL,
  PRIMARY KEY (Id),
  KEY Id_Productos_idx (Id_Productos),
  CONSTRAINT Id_Productos FOREIGN KEY (Id_Productos) REFERENCES productos (Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";

            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Activo>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Fecha, Id_Categoria, Id_Productos, Codigo_Contable, No_Serie, Modelo, Marca, Nombre, Precio, IVA, C_Total, Id_Movimientos, Id_Salida, Id_Entrada, No_Factura, Fecha_Factura FROM activo ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Activo>();
            while (await reader.ReadAsync())
            {
                results.Add(new Activo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    IdCategoria = reader.IsDBNull(reader.GetOrdinal("Id_Categoria")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Categoria")),
                    IdProductos = reader.IsDBNull(reader.GetOrdinal("Id_Productos")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Productos")),
                    CodigoContable = reader.IsDBNull(reader.GetOrdinal("Codigo_Contable")) ? null : reader.GetString(reader.GetOrdinal("Codigo_Contable")),
                    NoSerie = reader.IsDBNull(reader.GetOrdinal("No_Serie")) ? null : reader.GetString(reader.GetOrdinal("No_Serie")),
                    Modelo = reader.IsDBNull(reader.GetOrdinal("Modelo")) ? null : reader.GetString(reader.GetOrdinal("Modelo")),
                    Marca = reader.IsDBNull(reader.GetOrdinal("Marca")) ? null : reader.GetString(reader.GetOrdinal("Marca")),
                    Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                    Precio = reader.IsDBNull(reader.GetOrdinal("Precio")) ? null : reader.GetDecimal(reader.GetOrdinal("Precio")),
                    IVA = reader.IsDBNull(reader.GetOrdinal("IVA")) ? null : reader.GetDecimal(reader.GetOrdinal("IVA")),
                    CTotal = reader.IsDBNull(reader.GetOrdinal("C_Total")) ? null : reader.GetDecimal(reader.GetOrdinal("C_Total")),
                    IdMovimientos = reader.IsDBNull(reader.GetOrdinal("Id_Movimientos")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Movimientos")),
                    IdSalida = reader.IsDBNull(reader.GetOrdinal("Id_Salida")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Salida")),
                    IdEntrada = reader.IsDBNull(reader.GetOrdinal("Id_Entrada")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Entrada")),
                    NoFactura = reader.IsDBNull(reader.GetOrdinal("No_Factura")) ? null : reader.GetInt32(reader.GetOrdinal("No_Factura")),
                    FechaFactura = reader.IsDBNull(reader.GetOrdinal("Fecha_Factura")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_Factura"))
                });
            }

            return results;
        }

        public async Task<Activo?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Fecha, Id_Categoria, Id_Productos, Codigo_Contable, No_Serie, Modelo, Marca, Nombre, Precio, IVA, C_Total, Id_Movimientos, Id_Salida, Id_Entrada, No_Factura, Fecha_Factura FROM activo WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Activo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha")),
                IdCategoria = reader.IsDBNull(reader.GetOrdinal("Id_Categoria")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Categoria")),
                IdProductos = reader.IsDBNull(reader.GetOrdinal("Id_Productos")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Productos")),
                CodigoContable = reader.IsDBNull(reader.GetOrdinal("Codigo_Contable")) ? null : reader.GetString(reader.GetOrdinal("Codigo_Contable")),
                NoSerie = reader.IsDBNull(reader.GetOrdinal("No_Serie")) ? null : reader.GetString(reader.GetOrdinal("No_Serie")),
                Modelo = reader.IsDBNull(reader.GetOrdinal("Modelo")) ? null : reader.GetString(reader.GetOrdinal("Modelo")),
                Marca = reader.IsDBNull(reader.GetOrdinal("Marca")) ? null : reader.GetString(reader.GetOrdinal("Marca")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                Precio = reader.IsDBNull(reader.GetOrdinal("Precio")) ? null : reader.GetDecimal(reader.GetOrdinal("Precio")),
                IVA = reader.IsDBNull(reader.GetOrdinal("IVA")) ? null : reader.GetDecimal(reader.GetOrdinal("IVA")),
                CTotal = reader.IsDBNull(reader.GetOrdinal("C_Total")) ? null : reader.GetDecimal(reader.GetOrdinal("C_Total")),
                IdMovimientos = reader.IsDBNull(reader.GetOrdinal("Id_Movimientos")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Movimientos")),
                IdSalida = reader.IsDBNull(reader.GetOrdinal("Id_Salida")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Salida")),
                IdEntrada = reader.IsDBNull(reader.GetOrdinal("Id_Entrada")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Entrada")),
                NoFactura = reader.IsDBNull(reader.GetOrdinal("No_Factura")) ? null : reader.GetInt32(reader.GetOrdinal("No_Factura")),
                FechaFactura = reader.IsDBNull(reader.GetOrdinal("Fecha_Factura")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_Factura"))
            };
        }

        public async Task CreateAsync(Activo activo)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO activo (Fecha, Id_Categoria, Id_Productos, Codigo_Contable, No_Serie, Modelo, Marca, Nombre, Precio, IVA, C_Total, Id_Movimientos, Id_Salida, Id_Entrada, No_Factura, Fecha_Factura)
VALUES (@fecha, @idCategoria, @idProductos, @codigoContable, @noSerie, @modelo, @marca, @nombre, @precio, @iva, @cTotal, @idMovimientos, @idSalida, @idEntrada, @noFactura, @fechaFactura);";
            command.Parameters.AddWithValue("@fecha", activo.Fecha.HasValue ? activo.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idCategoria", activo.IdCategoria.HasValue ? activo.IdCategoria.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProductos", activo.IdProductos.HasValue ? activo.IdProductos.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@codigoContable", activo.CodigoContable ?? string.Empty);
            command.Parameters.AddWithValue("@noSerie", activo.NoSerie ?? string.Empty);
            command.Parameters.AddWithValue("@modelo", activo.Modelo ?? string.Empty);
            command.Parameters.AddWithValue("@marca", activo.Marca ?? string.Empty);
            command.Parameters.AddWithValue("@nombre", activo.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@precio", activo.Precio.HasValue ? activo.Precio.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@iva", activo.IVA.HasValue ? activo.IVA.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@cTotal", activo.CTotal.HasValue ? activo.CTotal.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idMovimientos", activo.IdMovimientos.HasValue ? activo.IdMovimientos.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idSalida", activo.IdSalida.HasValue ? activo.IdSalida.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idEntrada", activo.IdEntrada.HasValue ? activo.IdEntrada.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@noFactura", activo.NoFactura.HasValue ? activo.NoFactura.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fechaFactura", activo.FechaFactura.HasValue ? activo.FechaFactura.Value : (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Activo activo)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE activo SET Fecha = @fecha, Id_Categoria = @idCategoria, Id_Productos = @idProductos, Codigo_Contable = @codigoContable, No_Serie = @noSerie, Modelo = @modelo, Marca = @marca, Nombre = @nombre, Precio = @precio, IVA = @iva, C_Total = @cTotal, Id_Movimientos = @idMovimientos, Id_Salida = @idSalida, Id_Entrada = @idEntrada, No_Factura = @noFactura, Fecha_Factura = @fechaFactura WHERE Id = @id;";
            command.Parameters.AddWithValue("@fecha", activo.Fecha.HasValue ? activo.Fecha.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idCategoria", activo.IdCategoria.HasValue ? activo.IdCategoria.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProductos", activo.IdProductos.HasValue ? activo.IdProductos.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@codigoContable", activo.CodigoContable ?? string.Empty);
            command.Parameters.AddWithValue("@noSerie", activo.NoSerie ?? string.Empty);
            command.Parameters.AddWithValue("@modelo", activo.Modelo ?? string.Empty);
            command.Parameters.AddWithValue("@marca", activo.Marca ?? string.Empty);
            command.Parameters.AddWithValue("@nombre", activo.Nombre ?? string.Empty);
            command.Parameters.AddWithValue("@precio", activo.Precio.HasValue ? activo.Precio.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@iva", activo.IVA.HasValue ? activo.IVA.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@cTotal", activo.CTotal.HasValue ? activo.CTotal.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idMovimientos", activo.IdMovimientos.HasValue ? activo.IdMovimientos.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idSalida", activo.IdSalida.HasValue ? activo.IdSalida.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idEntrada", activo.IdEntrada.HasValue ? activo.IdEntrada.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@noFactura", activo.NoFactura.HasValue ? activo.NoFactura.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@fechaFactura", activo.FechaFactura.HasValue ? activo.FechaFactura.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@id", activo.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM activo WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
