using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using kardex_Web.Models;
using kardex_Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kardex_Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public LoginModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new LoginInput();

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _usuarioService.GetByUsernameAsync(Input.UserName ?? string.Empty);
            if (user is null || user.Eliminado || !IsValidPassword(user.Contraseña, Input.Password))
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UsuarioNombre ?? user.Nombre ?? string.Empty),
            };

            if (!string.IsNullOrWhiteSpace(user.Cargo))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Cargo));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return LocalRedirect(Url.Content("~/"));
        }

        public sealed record LoginInput
        {
            [Required(ErrorMessage = "El usuario es obligatorio.")]
            public string? UserName { get; init; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            public string? Password { get; init; }

            public bool RememberMe { get; init; }
        }

        private static bool IsValidPassword(string? storedPassword, string? enteredPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword) || string.IsNullOrWhiteSpace(enteredPassword))
            {
                return false;
            }

            try
            {
                var decrypted = Encriptar_Contrasena.Decrypt(storedPassword);
                return decrypted == enteredPassword;
            }
            catch
            {
                return storedPassword == enteredPassword;
            }
        }
    }
}
