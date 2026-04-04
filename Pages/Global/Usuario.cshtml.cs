using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages.Global
{
    public class UsuarioModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IList<Usuario> Usuarios { get; private set; } = new List<Usuario>();

        [BindProperty]
        public Usuario Input { get; set; } = new Usuario();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var usuario = await _usuarioService.GetUsuarioAsync(id.Value);
                if (usuario != null)
                {
                    Input = usuario;
                }
            }

            Usuarios = (await _usuarioService.GetUsuariosAsync()).ToList();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Usuarios = (await _usuarioService.GetUsuariosAsync()).ToList();
                return Page();
            }

            if (Input.Id > 0)
            {
                await _usuarioService.UpdateUsuarioAsync(Input);
                StatusMessage = "Usuario actualizado correctamente.";
            }
            else
            {
                await _usuarioService.CreateUsuarioAsync(Input);
                StatusMessage = "Usuario creado correctamente.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _usuarioService.SoftDeleteUsuarioAsync(id);
            StatusMessage = "Usuario eliminado (soft delete).";
            return RedirectToPage();
        }
    }
}
