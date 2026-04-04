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
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; color: #212529; }}
        h1 {{ font-size: 1.5rem; margin-bottom: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        table, th, td {{ border: 1px solid #444; }}
        th, td {{ padding: 8px; text-align: left; }}
        th {{ background: #f2f2f2; }}
        .no-print {{ display: none; }}
        @media print {{
            body {{ margin: 0; }}
            table {{ page-break-inside: avoid; }}
            tr {{ page-break-inside: avoid; page-break-after: auto; }}
        }}
    </style>
</head>
<body>
    <h1>{encodedTitle}</h1>
    {htmlContent}
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
