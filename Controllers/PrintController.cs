using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;

namespace kardex_Web.Controllers
{
    [ApiController]
    [Route("print")]
    public class PrintController : ControllerBase
    {
        [HttpPost("html")]
        [IgnoreAntiforgeryToken]
        public IActionResult Html([FromForm] PrintHtmlRequest request)
        {
            var htmlContent = request.Html ?? string.Empty;
            var title = string.IsNullOrWhiteSpace(request.Title) ? "Imprimir" : request.Title.Trim();
            var encodedTitle = HtmlEncoder.Default.Encode(title);

            var output = $@"<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>{encodedTitle}</title>
    <link rel='stylesheet' href='/css/print.css' />
</head>
<body>
    <header class='print-header'>
        <img class='logo' src='/img/Escudo.png' alt='Logo izquierdo' />
        <div class='title-block'>
            <h1>{encodedTitle}</h1>
            <div class='subtitle'>Documento generado automáticamente</div>
        </div>
        <img class='logo' src='/img/Vamos%20Adelante.png' alt='Logo derecho' />
    </header>
    <div class='table-wrapper'>
        {htmlContent}
    </div>
    <script>
        window.onload = function() {{
            window.print();
        }};
    </script>
</body>
</html>";

            return Content(output, "text/html");
        }

        public sealed record PrintHtmlRequest(string Html, string? Title);
    }
}
