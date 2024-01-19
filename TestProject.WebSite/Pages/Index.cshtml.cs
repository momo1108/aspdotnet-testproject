using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestProject.WebSite.Models;
using TestProject.WebSite.Services;

namespace TestProject.WebSite.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, JsonFileProductService productService) : PageModel
    {
        public JsonFileProductService ProductService = productService;
        public IEnumerable<Product> Products { get; private set; }

        public void OnGet()
        {
            Products = ProductService.GetProducts();
        }
    }
}
