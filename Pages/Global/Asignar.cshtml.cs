using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class AsignarModel : PageModel
    {
        private readonly AsignarService _service;

        public AsignarModel(AsignarService service)
        {
            _service = service;
        }

        public IList<Asignar> Asignaciones { get; private set; } = new List<Asignar>();

        [BindProperty]
        public Asignar Input { get; set; } = new Asignar();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var asignar = await _service.GetByIdAsync(id.Value);
                if (asignar != null)
                {
                    Input = asignar;
                }
            }

            Asignaciones = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Asignaciones = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Asignación actualizada correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Asignación creada correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Asignación eliminada correctamente.";
            return RedirectToPage();
        }
    }
}
