using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestProject.WebSite.Models;
using TestProject.WebSite.Services;

namespace TestProject.WebSite.Controllers
{
    [Route("[controller]")] // 파일이름으로 기본설정 되는듯.
    [ApiController]
    public class ProductsController(JsonFileProductService productService) : ControllerBase // ControllerBase 자식 클래스
    {
        public JsonFileProductService ProductService { get; } = productService;

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return ProductService.GetProducts();
        }

        //[Route("Rate")]
        //[HttpPatch]
        //public ActionResult Get(
        //    [FromBody] string productId, 
        //    [FromBody] int rating
        //)
        //{
        //    ProductService.AddRating(productId, rating);
        //    return Ok();
        //}

        [Route("Rate")]
        [HttpGet]
        public ActionResult Get(
            [FromQuery] string productId, 
            [FromQuery] int rating
        )
        {
            ProductService.AddRating(productId, rating);
            return Ok();
        }
    }
}
