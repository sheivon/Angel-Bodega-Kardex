using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class ProveedorModel : PageModel
    {
        private readonly ProveedorService _service;

        public ProveedorModel(ProveedorService service)
        {
            _service = service;
        }

        public IList<Proveedor> Proveedores { get; private set; } = new List<Proveedor>();

        [BindProperty]
        public Proveedor Input { get; set; } = new Proveedor();

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

            Proveedores = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Proveedores = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Proveedor actualizado correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Proveedor creado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Proveedor eliminado correctamente.";
            return RedirectToPage();
        }
    }
}
