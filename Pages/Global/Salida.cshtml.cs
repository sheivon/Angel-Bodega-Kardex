using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class SalidaModel : PageModel
    {
        private readonly SalidaService _service;

        public SalidaModel(SalidaService service)
        {
            _service = service;
        }

        public IList<Salida> Salidas { get; private set; } = new List<Salida>();

        [BindProperty]
        public Salida Input { get; set; } = new Salida();

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

            Salidas = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Salidas = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Salida actualizada correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Salida creada correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Salida eliminada correctamente.";
            return RedirectToPage();
        }
    }
}
