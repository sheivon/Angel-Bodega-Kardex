using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Registrar
{
    public class MovimientoModel : PageModel
    {
        private readonly MovimientoService _service;

        public MovimientoModel(MovimientoService service)
        {
            _service = service;
        }

        public IList<Movimiento> Movimientos { get; private set; } = new List<Movimiento>();

        [BindProperty]
        public Movimiento Input { get; set; } = new Movimiento();

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

            Movimientos = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Movimientos = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Movimiento actualizado correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Movimiento creado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
            StatusMessage = "Movimiento eliminado correctamente.";
            return RedirectToPage();
        }
    }
}
