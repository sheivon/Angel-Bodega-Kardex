using System.IO;
using System.Linq;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using MySqlConnector;
using System.Xml.Linq;

namespace kardex_Web.Pages.Global
{
    public class SetupModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly UsuarioService _usuarioService;
        private readonly UnidadMedidaService _unidadMedidaService;
        private readonly AutorizaService _autorizaService;
        private readonly CategoriaService _categoriaService;
        private readonly AreaTrabajoService _areaTrabajoService;
        private readonly ProveedorService _proveedorService;
        private readonly ProyectoService _proyectoService;
        private readonly RetiraService _retiraService;
        private readonly SalidaService _salidaService;
        private readonly OrdenCompraService _ordenCompraService;
        private readonly ProductoService _productoService;
        private readonly EntradaService _entradaService;
        private readonly MovimientoService _movimientoService;
        private readonly PeriodoService _periodoService;
        private readonly AsignarService _asignarService;

        public SetupModel(IConfiguration configuration, IWebHostEnvironment environment, UsuarioService usuarioService, UnidadMedidaService unidadMedidaService, AutorizaService autorizaService, CategoriaService categoriaService, AreaTrabajoService areaTrabajoService, ProveedorService proveedorService, ProyectoService proyectoService, RetiraService retiraService, SalidaService salidaService, OrdenCompraService ordenCompraService, ProductoService productoService, EntradaService entradaService, MovimientoService movimientoService, PeriodoService periodoService, AsignarService asignarService)
        {
            _configuration = configuration;
            _environment = environment;
            _usuarioService = usuarioService;
            _unidadMedidaService = unidadMedidaService;
            _autorizaService = autorizaService;
            _categoriaService = categoriaService;
            _areaTrabajoService = areaTrabajoService;
            _proveedorService = proveedorService;
            _proyectoService = proyectoService;
            _retiraService = retiraService;
            _salidaService = salidaService;
            _ordenCompraService = ordenCompraService;
            _productoService = productoService;
            _entradaService = entradaService;
            _movimientoService = movimientoService;
            _periodoService = periodoService;
            _asignarService = asignarService;
        }

        [BindProperty]
        public string ConnectionString { get; set; } = string.Empty;

        public string? CurrentDatabase { get; set; }

        public string? ConnectionStringMessage { get; set; }

        public string? DatabaseMessage { get; set; }

        public List<TableInfo> Tables { get; set; } = new();

        public List<SetupResult> Results { get; set; } = new();

        public async Task OnGetAsync()
        {
            ConnectionString = GetCurrentConnectionString() ?? string.Empty;
            CurrentDatabase = GetDatabaseName(ConnectionString);
            Tables = await GetTableStatusAsync(ConnectionString);
        }

        public async Task OnPostAllAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Usuarios", _usuarioService.EnsureTableExistsAsync),
                await RunSetupAsync("Unidad de Medida", _unidadMedidaService.EnsureTableExistsAsync),
                await RunSetupAsync("Autoriza", _autorizaService.EnsureTableExistsAsync),
                await RunSetupAsync("Categoría", _categoriaService.EnsureTableExistsAsync),
                await RunSetupAsync("Área de Trabajo", _areaTrabajoService.EnsureTableExistsAsync),
                await RunSetupAsync("Proveedor", _proveedorService.EnsureTableExistsAsync),
                await RunSetupAsync("Proyecto", _proyectoService.EnsureTableExistsAsync),
                await RunSetupAsync("Periodo", _periodoService.EnsureTableExistsAsync),
                await RunSetupAsync("Asignar", _asignarService.EnsureTableExistsAsync),
                await RunSetupAsync("Retira", _retiraService.EnsureTableExistsAsync),
                await RunSetupAsync("Orden de Compra", _ordenCompraService.EnsureTableExistsAsync),
                await RunSetupAsync("Productos", _productoService.EnsureTableExistsAsync),
                await RunSetupAsync("Entrada", _entradaService.EnsureTableExistsAsync),
                await RunSetupAsync("Movimientos", _movimientoService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostUsuariosAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Usuarios", _usuarioService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostUnidadMedidaAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Unidad de Medida", _unidadMedidaService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostAutorizaAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Autoriza", _autorizaService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostCategoriaAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Categoría", _categoriaService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostAreaTrabajoAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Área de Trabajo", _areaTrabajoService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostProveedorAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Proveedor", _proveedorService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostProyectoAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Proyecto", _proyectoService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostPeriodoAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Periodo", _periodoService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostAsignarAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Asignar", _asignarService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostRetiraAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Retira", _retiraService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostSalidaAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Salida", _salidaService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostOrdenCompraAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Orden de Compra", _ordenCompraService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostProductoAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Productos", _productoService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostEntradaAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Entrada", _entradaService.EnsureTableExistsAsync)
            };
        }

        public async Task OnPostMovimientosAsync()
        {
            Results = new List<SetupResult>
            {
                await RunSetupAsync("Movimientos", _movimientoService.EnsureTableExistsAsync)
            };
        }

        public async Task<IActionResult> OnPostSaveConnectionStringAsync()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                ConnectionStringMessage = "La cadena de conexión no puede estar vacía.";
                Tables = await GetTableStatusAsync(GetCurrentConnectionString() ?? string.Empty);
                return Page();
            }

            await SaveConnectionStringAsync(ConnectionString);
            CurrentDatabase = GetDatabaseName(ConnectionString);
            ConnectionStringMessage = "Cadena de conexión guardada correctamente en web.config.";
            Tables = await GetTableStatusAsync(ConnectionString);
            return Page();
        }

        public async Task<IActionResult> OnPostCreateDatabaseAsync()
        {
            var currentConnection = GetCurrentConnectionString();
            if (string.IsNullOrWhiteSpace(currentConnection))
            {
                DatabaseMessage = "No se encontró una cadena de conexión MySql válida.";
                Tables = await GetTableStatusAsync(string.Empty);
                return Page();
            }

            var builder = new MySqlConnectionStringBuilder(currentConnection);
            var dbName = builder.Database;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                DatabaseMessage = "La cadena de conexión debe incluir el nombre de la base de datos.";
                Tables = await GetTableStatusAsync(currentConnection);
                return Page();
            }

            builder.Database = string.Empty;
            await using var connection = new MySqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            var safeDbName = dbName.Replace("`", "``");
            command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{safeDbName}` CHARACTER SET utf8mb3 COLLATE utf8_general_ci;";
            await command.ExecuteNonQueryAsync();

            CurrentDatabase = dbName;
            DatabaseMessage = $"Base de datos '{dbName}' creada o verificada correctamente.";
            Tables = await GetTableStatusAsync(currentConnection);
            return Page();
        }

        public async Task<IActionResult> OnPostRefreshTablesAsync()
        {
            ConnectionString = GetCurrentConnectionString() ?? string.Empty;
            CurrentDatabase = GetDatabaseName(ConnectionString);
            Tables = await GetTableStatusAsync(ConnectionString);
            return Page();
        }

        private string? GetCurrentConnectionString()
        {
            return _configuration.GetConnectionString("MySql");
        }

        private static string? GetDatabaseName(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                return builder.Database;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<List<TableInfo>> GetTableStatusAsync(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return new List<TableInfo>();
            }

            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                var schema = builder.Database;
                if (string.IsNullOrWhiteSpace(schema))
                {
                    return new List<TableInfo>();
                }

                await using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = @"SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA = @schema ORDER BY TABLE_NAME;";
                command.Parameters.AddWithValue("@schema", schema);

                var tables = new List<TableInfo>();
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tables.Add(new TableInfo(reader.GetString(0), true, "Existe"));
                }

                return tables;
            }
            catch
            {
                return new List<TableInfo>();
            }
        }

        private async Task SaveConnectionStringAsync(string connectionString)
        {
            var webConfigPath = Path.Combine(_environment.ContentRootPath, "web.config");
            XDocument doc;
            if (System.IO.File.Exists(webConfigPath))
            {
                doc = XDocument.Load(webConfigPath);
            }
            else
            {
                doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("configuration"));
            }

            var root = doc.Element("configuration");
            if (root == null)
            {
                root = new XElement("configuration");
                doc.Add(root);
            }

            var connectionStrings = root.Element("connectionStrings");
            if (connectionStrings == null)
            {
                connectionStrings = new XElement("connectionStrings");
                root.Add(connectionStrings);
            }

            var addElement = connectionStrings.Elements("add").FirstOrDefault(x => (string?)x.Attribute("name") == "MySql");
            if (addElement == null)
            {
                addElement = new XElement("add");
                addElement.SetAttributeValue("name", "MySql");
                addElement.SetAttributeValue("providerName", "MySql.Data.MySqlClient");
                connectionStrings.Add(addElement);
            }

            addElement.SetAttributeValue("connectionString", connectionString);
            addElement.SetAttributeValue("providerName", "MySql.Data.MySqlClient");

            doc.Save(webConfigPath);
        }

        private static async Task<SetupResult> RunSetupAsync(string entityName, Func<Task> setupAction)
        {
            try
            {
                await setupAction();
                return new SetupResult(entityName, true, "Tabla creada o verificada correctamente.");
            }
            catch (Exception ex)
            {
                return new SetupResult(entityName, false, ex.Message);
            }
        }

        public sealed record SetupResult(string EntityName, bool Success, string Message);
    }
}
