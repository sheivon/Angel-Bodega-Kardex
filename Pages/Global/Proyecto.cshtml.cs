using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class ProyectoModel : PageModel
    {
        private readonly ProyectoService _service;

        public ProyectoModel(ProyectoService service)
        {
            _service = service;
        }

        public IList<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

        [BindProperty]
        public Proyecto Input { get; set; } = new Proyecto();

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

            Proyectos = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Proyectos = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Proyecto actualizado correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Proyecto creado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Proyecto eliminado correctamente.";
            return RedirectToPage();
        }
    }
}
