using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class ProductoModel : PageModel
    {
        private readonly ProductoService _service;
        private readonly UnidadMedidaService _unidadMedidaService;

        public ProductoModel(ProductoService service, UnidadMedidaService unidadMedidaService)
        {
            _service = service;
            _unidadMedidaService = unidadMedidaService;
        }

        public IList<Producto> Productos { get; private set; } = new List<Producto>();
        public IList<UnidadMedida> UnidadMedidas { get; private set; } = new List<UnidadMedida>();

        [BindProperty]
        public Producto Input { get; set; } = new Producto();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item != null)
                {
                    Input = item;
                }
            }

            Productos = (await _service.GetAllAsync()).ToList();
            UnidadMedidas = (await _unidadMedidaService.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Productos = (await _service.GetAllAsync()).ToList();
                UnidadMedidas = (await _unidadMedidaService.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Producto actualizado correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Producto creado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
            StatusMessage = "Producto eliminado correctamente.";
            return RedirectToPage();
        }
    }
}
