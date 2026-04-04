using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class AreaTrabajoModel : PageModel
    {
        private readonly AreaTrabajoService _service;

        public AreaTrabajoModel(AreaTrabajoService service)
        {
            _service = service;
        }

        public IList<AreaTrabajo> AreasTrabajo { get; private set; } = new List<AreaTrabajo>();

        [BindProperty]
        public AreaTrabajo Input { get; set; } = new AreaTrabajo();

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

            AreasTrabajo = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                AreasTrabajo = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Área de trabajo actualizada correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Área de trabajo creada correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Área de trabajo eliminada correctamente.";
            return RedirectToPage();
        }
    }
}
