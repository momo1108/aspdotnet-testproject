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

        public void AddRating(string productId, int rating)
        {
            var products = GetProducts();

            // LINQ
            var query = products.First(p => p.Id == productId);

            if(query.Ratings == null)
            {
                query.Ratings = new int[] { rating };
            } else
            {
                query.Ratings = [.. query.Ratings, rating];
            }

            using(var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true,
                    }),
                    products
                );
            }
        }
    }
}
