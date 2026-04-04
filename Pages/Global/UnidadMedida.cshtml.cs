using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class UnidadMedidaModel : PageModel
    {
        private readonly UnidadMedidaService _service;

        public UnidadMedidaModel(UnidadMedidaService service)
        {
            _service = service;
        }

        public IList<UnidadMedida> Unidades { get; private set; } = new List<UnidadMedida>();

        [BindProperty]
        public UnidadMedida Input { get; set; } = new UnidadMedida();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var unidad = await _service.GetByIdAsync(id.Value);
                if (unidad != null)
                {
                    Input = unidad;
                }
            }

            Unidades = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Unidades = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Unidad de medida actualizada correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Unidad de medida creada correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
            StatusMessage = "Unidad de medida eliminada correctamente.";
            return RedirectToPage();
        }
    }
}
