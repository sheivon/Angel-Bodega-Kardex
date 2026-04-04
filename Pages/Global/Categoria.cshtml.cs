using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class CategoriaModel : PageModel
    {
        private readonly CategoriaService _service;

        public CategoriaModel(CategoriaService service)
        {
            _service = service;
        }

        public IList<Categoria> Categorias { get; private set; } = new List<Categoria>();

        [BindProperty]
        public Categoria Input { get; set; } = new Categoria();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var categoria = await _service.GetByIdAsync(id.Value);
                if (categoria != null)
                {
                    Input = categoria;
                }
            }

            Categorias = (await _service.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Categorias = (await _service.GetAllAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _service.UpdateAsync(Input);
                StatusMessage = "Categoría actualizada correctamente.";
            }
            else
            {
                await _service.CreateAsync(Input);
                StatusMessage = "Categoría creada correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.SoftDeleteAsync(id);
            StatusMessage = "Categoría eliminada correctamente.";
            return RedirectToPage();
        }
    }
}
