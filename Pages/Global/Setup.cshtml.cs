using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class SetupModel : PageModel
    {
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

        public SetupModel(UsuarioService usuarioService, UnidadMedidaService unidadMedidaService, AutorizaService autorizaService, CategoriaService categoriaService, AreaTrabajoService areaTrabajoService, ProveedorService proveedorService, ProyectoService proyectoService, RetiraService retiraService, SalidaService salidaService, OrdenCompraService ordenCompraService, ProductoService productoService, EntradaService entradaService, MovimientoService movimientoService)
        {
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
        }

        public List<SetupResult> Results { get; set; } = new();

        public void OnGet()
        {
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
                await RunSetupAsync("Retira", _retiraService.EnsureTableExistsAsync),
                await RunSetupAsync("Salida", _salidaService.EnsureTableExistsAsync),
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
