using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace kardex_Web.Controllers
{
    [ApiController]
    [Route("api/db")]
    public class DatabaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DatabaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("connection")]
        public IActionResult GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("MySql");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return NotFound(new { Message = "MySql connection string not found in configuration." });
            }

            return Ok(new { Name = "MySql", ConnectionString = connectionString });
        }

        [HttpPost("connection")]
        public IActionResult SaveConnectionString([FromBody] ConnectionStringRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                return BadRequest(new { Message = "Connection string is required." });
            }

            var filePath = GetWebConfigPath();
            SaveConnectionStringToWebConfig(filePath, "MySql", request.ConnectionString, "MySql.Data.MySqlClient");

            return Ok(new { Message = "Connection string saved successfully.", File = filePath });
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestConnection()
        {
            var connectionString = _configuration.GetConnectionString("MySql");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return NotFound(new { Message = "MySql connection string not found in configuration." });
            }

            try
            {
                await using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                return Ok(new { Success = true, Database = connection.Database, ServerVersion = connection.ServerVersion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        private static string GetWebConfigPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "web.config");
        }

        private static void SaveConnectionStringToWebConfig(string filePath, string name, string connectionString, string providerName)
        {
            XDocument document;
            if (System.IO.File.Exists(filePath))
            {
                document = XDocument.Load(filePath);
            }
            else
            {
                document = new XDocument(new XElement("configuration"));
            }

            var root = document.Element("configuration");
            if (root == null)
            {
                root = new XElement("configuration");
                document.Add(root);
            }

            var connectionStrings = root.Element("connectionStrings");
            if (connectionStrings == null)
            {
                connectionStrings = new XElement("connectionStrings");
                root.Add(connectionStrings);
            }

            XElement? element = connectionStrings.Elements("add")
                .FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), name, StringComparison.OrdinalIgnoreCase));

            if (element == null)
            {
                element = new XElement("add");
                connectionStrings.Add(element);
            }

            element.SetAttributeValue("name", name);
            element.SetAttributeValue("connectionString", connectionString);
            element.SetAttributeValue("providerName", providerName);

            document.Save(filePath);
        }

        public sealed record ConnectionStringRequest(string ConnectionString);
    }
}
