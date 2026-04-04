using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace kardex_Web.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        [HttpGet("general")]
        public IActionResult GetGeneralConfig()
        {
            var config = ReadGeneralSettings();
            return Ok(config);
        }

        [HttpPost("general")]
        public IActionResult SaveGeneralConfig([FromBody] GeneralConfigModel? request)
        {
            if (request is null)
            {
                return BadRequest(new { Message = "Request body is required." });
            }

            SaveGeneralSettings(request);
            return Ok(new { Message = "General configuration saved successfully." });
        }

        private static GeneralConfigModel ReadGeneralSettings()
        {
            var filePath = GetWebConfigPath();
            if (!System.IO.File.Exists(filePath))
            {
                return new GeneralConfigModel();
            }

            var document = XDocument.Load(filePath);
            var appSettings = document.Root?.Element("appSettings");
            return new GeneralConfigModel
            {
                NombreAlcaldia = ReadAppSettingValue(appSettings, "NombreAlcaldia"),
                IvaPercent = ReadAppSettingValue(appSettings, "Iva"),
                ConsecutivoEntrada = ReadAppSettingValue(appSettings, "ConsecutivoEntrada"),
                ConsecutivoSalida = ReadAppSettingValue(appSettings, "ConsecutivoSalida")
            };
        }

        private static void SaveGeneralSettings(GeneralConfigModel settings)
        {
            var filePath = GetWebConfigPath();
            XDocument document;
            if (System.IO.File.Exists(filePath))
            {
                document = XDocument.Load(filePath);
            }
            else
            {
                document = new XDocument(new XElement("configuration"));
            }

            var root = document.Root ?? new XElement("configuration");
            if (document.Root == null)
            {
                document.Add(root);
            }

            var appSettings = root.Element("appSettings");
            if (appSettings == null)
            {
                appSettings = new XElement("appSettings");
                root.Add(appSettings);
            }

            SetAppSettingValue(appSettings, "NombreAlcaldia", settings.NombreAlcaldia ?? string.Empty);
            SetAppSettingValue(appSettings, "Iva", settings.IvaPercent ?? string.Empty);
            SetAppSettingValue(appSettings, "ConsecutivoEntrada", settings.ConsecutivoEntrada ?? string.Empty);
            SetAppSettingValue(appSettings, "ConsecutivoSalida", settings.ConsecutivoSalida ?? string.Empty);

            document.Save(filePath);
        }

        private static string? ReadAppSettingValue(XElement? appSettings, string key)
        {
            return appSettings?.Elements("add")
                .Where(x => string.Equals((string?)x.Attribute("key"), key, StringComparison.OrdinalIgnoreCase))
                .Select(x => (string?)x.Attribute("value"))
                .FirstOrDefault();
        }

        private static void SetAppSettingValue(XElement appSettings, string key, string value)
        {
            var element = appSettings.Elements("add")
                .FirstOrDefault(x => string.Equals((string?)x.Attribute("key"), key, StringComparison.OrdinalIgnoreCase));

            if (element == null)
            {
                element = new XElement("add");
                appSettings.Add(element);
            }

            element.SetAttributeValue("key", key);
            element.SetAttributeValue("value", value);
        }

        private static string GetWebConfigPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "web.config");
        }

        public sealed record GeneralConfigModel
        {
            public string? NombreAlcaldia { get; init; }
            public string? IvaPercent { get; init; }
            public string? ConsecutivoEntrada { get; init; }
            public string? ConsecutivoSalida { get; init; }
        }
    }
}
