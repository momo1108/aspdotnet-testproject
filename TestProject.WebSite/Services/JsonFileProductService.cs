using System.Text.Json;
using TestProject.WebSite.Models;

namespace TestProject.WebSite.Services
{
    public class JsonFileProductService(IWebHostEnvironment webHostEnvironment)
    {
        public IWebHostEnvironment WebHostEnvironment { get; } = webHostEnvironment;

        private string JsonFileName { get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json"); } }

        public IEnumerable<Product> GetProducts()
        {
            using(var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<Product[]>(jsonFileReader.ReadToEnd(), 
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
            }
        }
    }
}
