using MySqlConnector;
using kardex_Web.Models;

namespace kardex_Web.Services
{
    public class MovimientoService
    {
        private readonly string _connectionString;

        public MovimientoService(IConfiguration configuration)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS movimientos (
  Id INT NOT NULL AUTO_INCREMENT,
  Tipo_ES VARCHAR(2) DEFAULT NULL,
  Id_Ord_Compra INT DEFAULT NULL,
  Id_Proyecto INT DEFAULT NULL,
  Detalle VARCHAR(150) DEFAULT NULL,
  Fecha_ES DATE DEFAULT NULL,
  Id_Salida INT DEFAULT NULL,
  Id_Entrada INT DEFAULT NULL,
  Id_Producto INT DEFAULT NULL,
  E_Cantidad DECIMAL(15,2) DEFAULT NULL,
  S_Cantidad DECIMAL(15,2) DEFAULT NULL,
  Precio DECIMAL(15,2) DEFAULT NULL,
  IVA DECIMAL(15,2) DEFAULT NULL,
  Id_Area_Trabajo INT DEFAULT NULL,
  PRIMARY KEY (Id),
  KEY Id_Salida_idx (Id_Salida),
  KEY Id_Entrada_idx (Id_Entrada),
  KEY Id_Ord_Compra_idx (Id_Ord_Compra),
  KEY Id_Proyecto_idx (Id_Proyecto),
  KEY Id_Producto_idx (Id_Producto),
  KEY Id_Area_Trabajo_idx (Id_Area_Trabajo),
  CONSTRAINT Id_Entrada FOREIGN KEY (Id_Entrada) REFERENCES entrada (Id),
  CONSTRAINT Id_Salida FOREIGN KEY (Id_Salida) REFERENCES salida (Id),
  CONSTRAINT Id_Ord_Compra FOREIGN KEY (Id_Ord_Compra) REFERENCES orden_compra (Id),
  CONSTRAINT Id_Proyecto FOREIGN KEY (Id_Proyecto) REFERENCES proyecto (Id),
  CONSTRAINT Id_Producto FOREIGN KEY (Id_Producto) REFERENCES productos (Id),
  CONSTRAINT Id_Area_Trabajo FOREIGN KEY (Id_Area_Trabajo) REFERENCES area_trabajo (Id)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;";

            await command.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Movimiento>> GetAllAsync()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Tipo_ES, Id_Ord_Compra, Id_Proyecto, Detalle, Fecha_ES, Id_Salida, Id_Entrada, Id_Producto, E_Cantidad, S_Cantidad, Precio, IVA, Id_Area_Trabajo FROM movimientos ORDER BY Id;";

            await using var reader = await command.ExecuteReaderAsync();
            var results = new List<Movimiento>();
            while (await reader.ReadAsync())
            {
                results.Add(new Movimiento
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TipoES = reader.IsDBNull(reader.GetOrdinal("Tipo_ES")) ? null : reader.GetString(reader.GetOrdinal("Tipo_ES")),
                    IdOrdCompra = reader.IsDBNull(reader.GetOrdinal("Id_Ord_Compra")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Ord_Compra")),
                    IdProyecto = reader.IsDBNull(reader.GetOrdinal("Id_Proyecto")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Proyecto")),
                    Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString(reader.GetOrdinal("Detalle")),
                    FechaES = reader.IsDBNull(reader.GetOrdinal("Fecha_ES")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_ES")),
                    IdSalida = reader.IsDBNull(reader.GetOrdinal("Id_Salida")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Salida")),
                    IdEntrada = reader.IsDBNull(reader.GetOrdinal("Id_Entrada")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Entrada")),
                    IdProducto = reader.IsDBNull(reader.GetOrdinal("Id_Producto")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Producto")),
                    ECantidad = reader.IsDBNull(reader.GetOrdinal("E_Cantidad")) ? null : reader.GetDecimal(reader.GetOrdinal("E_Cantidad")),
                    SCantidad = reader.IsDBNull(reader.GetOrdinal("S_Cantidad")) ? null : reader.GetDecimal(reader.GetOrdinal("S_Cantidad")),
                    Precio = reader.IsDBNull(reader.GetOrdinal("Precio")) ? null : reader.GetDecimal(reader.GetOrdinal("Precio")),
                    IVA = reader.IsDBNull(reader.GetOrdinal("IVA")) ? null : reader.GetDecimal(reader.GetOrdinal("IVA")),
                    IdAreaTrabajo = reader.IsDBNull(reader.GetOrdinal("Id_Area_Trabajo")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Area_Trabajo"))
                });
            }

            return results;
        }

        public async Task<Movimiento?> GetByIdAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Id, Tipo_ES, Id_Ord_Compra, Id_Proyecto, Detalle, Fecha_ES, Id_Salida, Id_Entrada, Id_Producto, E_Cantidad, S_Cantidad, Precio, IVA, Id_Area_Trabajo FROM movimientos WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Movimiento
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TipoES = reader.IsDBNull(reader.GetOrdinal("Tipo_ES")) ? null : reader.GetString(reader.GetOrdinal("Tipo_ES")),
                IdOrdCompra = reader.IsDBNull(reader.GetOrdinal("Id_Ord_Compra")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Ord_Compra")),
                IdProyecto = reader.IsDBNull(reader.GetOrdinal("Id_Proyecto")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Proyecto")),
                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString(reader.GetOrdinal("Detalle")),
                FechaES = reader.IsDBNull(reader.GetOrdinal("Fecha_ES")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha_ES")),
                IdSalida = reader.IsDBNull(reader.GetOrdinal("Id_Salida")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Salida")),
                IdEntrada = reader.IsDBNull(reader.GetOrdinal("Id_Entrada")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Entrada")),
                IdProducto = reader.IsDBNull(reader.GetOrdinal("Id_Producto")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Producto")),
                ECantidad = reader.IsDBNull(reader.GetOrdinal("E_Cantidad")) ? null : reader.GetDecimal(reader.GetOrdinal("E_Cantidad")),
                SCantidad = reader.IsDBNull(reader.GetOrdinal("S_Cantidad")) ? null : reader.GetDecimal(reader.GetOrdinal("S_Cantidad")),
                Precio = reader.IsDBNull(reader.GetOrdinal("Precio")) ? null : reader.GetDecimal(reader.GetOrdinal("Precio")),
                IVA = reader.IsDBNull(reader.GetOrdinal("IVA")) ? null : reader.GetDecimal(reader.GetOrdinal("IVA")),
                IdAreaTrabajo = reader.IsDBNull(reader.GetOrdinal("Id_Area_Trabajo")) ? null : reader.GetInt32(reader.GetOrdinal("Id_Area_Trabajo"))
            };
        }

        public async Task CreateAsync(Movimiento movimiento)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO movimientos (Tipo_ES, Id_Ord_Compra, Id_Proyecto, Detalle, Fecha_ES, Id_Salida, Id_Entrada, Id_Producto, E_Cantidad, S_Cantidad, Precio, IVA, Id_Area_Trabajo)
VALUES (@tipoES, @idOrdCompra, @idProyecto, @detalle, @fechaES, @idSalida, @idEntrada, @idProducto, @eCantidad, @sCantidad, @precio, @iva, @idAreaTrabajo);";
            command.Parameters.AddWithValue("@tipoES", movimiento.TipoES ?? string.Empty);
            command.Parameters.AddWithValue("@idOrdCompra", movimiento.IdOrdCompra.HasValue ? movimiento.IdOrdCompra.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProyecto", movimiento.IdProyecto.HasValue ? movimiento.IdProyecto.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@detalle", movimiento.Detalle ?? string.Empty);
            command.Parameters.AddWithValue("@fechaES", movimiento.FechaES.HasValue ? movimiento.FechaES.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idSalida", movimiento.IdSalida.HasValue ? movimiento.IdSalida.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idEntrada", movimiento.IdEntrada.HasValue ? movimiento.IdEntrada.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProducto", movimiento.IdProducto.HasValue ? movimiento.IdProducto.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@eCantidad", movimiento.ECantidad.HasValue ? movimiento.ECantidad.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@sCantidad", movimiento.SCantidad.HasValue ? movimiento.SCantidad.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@precio", movimiento.Precio.HasValue ? movimiento.Precio.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@iva", movimiento.IVA.HasValue ? movimiento.IVA.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idAreaTrabajo", movimiento.IdAreaTrabajo.HasValue ? movimiento.IdAreaTrabajo.Value : (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Movimiento movimiento)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE movimientos SET Tipo_ES = @tipoES, Id_Ord_Compra = @idOrdCompra, Id_Proyecto = @idProyecto, Detalle = @detalle, Fecha_ES = @fechaES, Id_Salida = @idSalida, Id_Entrada = @idEntrada, Id_Producto = @idProducto, E_Cantidad = @eCantidad, S_Cantidad = @sCantidad, Precio = @precio, IVA = @iva, Id_Area_Trabajo = @idAreaTrabajo WHERE Id = @id;";
            command.Parameters.AddWithValue("@tipoES", movimiento.TipoES ?? string.Empty);
            command.Parameters.AddWithValue("@idOrdCompra", movimiento.IdOrdCompra.HasValue ? movimiento.IdOrdCompra.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProyecto", movimiento.IdProyecto.HasValue ? movimiento.IdProyecto.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@detalle", movimiento.Detalle ?? string.Empty);
            command.Parameters.AddWithValue("@fechaES", movimiento.FechaES.HasValue ? movimiento.FechaES.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idSalida", movimiento.IdSalida.HasValue ? movimiento.IdSalida.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idEntrada", movimiento.IdEntrada.HasValue ? movimiento.IdEntrada.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idProducto", movimiento.IdProducto.HasValue ? movimiento.IdProducto.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@eCantidad", movimiento.ECantidad.HasValue ? movimiento.ECantidad.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@sCantidad", movimiento.SCantidad.HasValue ? movimiento.SCantidad.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@precio", movimiento.Precio.HasValue ? movimiento.Precio.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@iva", movimiento.IVA.HasValue ? movimiento.IVA.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@idAreaTrabajo", movimiento.IdAreaTrabajo.HasValue ? movimiento.IdAreaTrabajo.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@id", movimiento.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM movimientos WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
