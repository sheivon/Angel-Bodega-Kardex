using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class OrdenCompraModel : PageModel
    {
        private readonly OrdenCompraService _service;

        public OrdenCompraModel(OrdenCompraService service)
        {
            _service = service;
        }

        public IList<OrdenCompra> OrdenesCompra { get; private set; } = new List<OrdenCompra>();

        [BindProperty]
        public OrdenCompra Input { get; set; } = new OrdenCompra();

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

            OrdenesCompra = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                OrdenesCompra = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Orden de compra actualizada correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Orden de compra creada correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Orden de compra eliminada correctamente.";
            return RedirectToPage();
        }
    }
}
