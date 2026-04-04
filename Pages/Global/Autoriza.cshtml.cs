using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class AutorizaModel : PageModel
    {
        private readonly AutorizaService _service;

        public AutorizaModel(AutorizaService service)
        {
            _service = service;
        }

        public IList<Autoriza> Autorizadores { get; private set; } = new List<Autoriza>();

        [BindProperty]
        public Autoriza Input { get; set; } = new Autoriza();

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

            Autorizadores = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Autorizadores = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Autoriza actualizado correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Autoriza creado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Autoriza eliminado correctamente.";
            return RedirectToPage();
        }
    }
}
