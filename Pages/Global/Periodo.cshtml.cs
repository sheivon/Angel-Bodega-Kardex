using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class PeriodoModel : PageModel
    {
        private readonly PeriodoService _service;

        public PeriodoModel(PeriodoService service)
        {
            _service = service;
        }

        public IList<Periodo> Periodos { get; private set; } = new List<Periodo>();

        [BindProperty]
        public Periodo Input { get; set; } = new Periodo();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var periodo = await _service.GetByIdAsync(id.Value);
                if (periodo != null)
                {
                    Input = periodo;
                }
            }

            Periodos = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Periodos = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Periodo actualizado correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Periodo creado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Periodo eliminado correctamente.";
            return RedirectToPage();
        }
    }
}
