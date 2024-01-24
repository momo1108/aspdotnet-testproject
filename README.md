# TestProject
ASP.NET�� �� ������Ʈ ������.

���� ������Ʈ ��ũ : https://github.com/dotnet-presentations/ContosoCrafts

## MVC

���ɻ縦 �и��ϴ� ������ ����̴�. Model �� ����� DB�� ������ �Ѵ�. View �� ���� �ƴ� ���� ���� ���� ������ �����͸� ����ϴ� ���Ҹ� �Ѵ�. Controller �� DB�� View ������ ������ �������� ������ �Ѵ�.

�� ������Ʈ ��Ʈ������ Models, Pages, Controllers ������ �����ϰ� ���� ���ҿ� �´� �ڵ带 �ۼ��Ѵ�. �� �ܿ� ���ø����̼� ������ ����� Ŀ���� ��ɵ��� Services ������ �ۼ��ߴ�.

���� ���� ������Ʈ�� products.json ���� �����͸� DB ������� ����ϰ� ������ ���� ���� Ratings ��� �׸� ����ڿ��� ��ȣ�ۿ��� ���� ������ �߰��ϴ� ���� ��ǥ�̴�.

�ε��� ���������� �ش� ������ ������� ������ְ�, �� �׸��� ��ư�� Ŭ�� �� �� ��ȸ�� ���� ����� �����ϴ� ����� ��쵵�� �����.

���⼭ Models �������� products.json �� ������ �ִ� �������� ������ �ڵ�� �ۼ������ �ϰ�,

Controllers ���������� ���� ������ api ����� ���� �����͸� �ҷ����� �����ؾ� ������ ���� ȯ���̱� ������ �ٷ� Services�� �ۼ��� �ڵ带 ����ϰ��ִ�.

Pages(Views) ���������� url ��ο� ���� �並 ������ ������ �ۼ��س��Ҵ�.

ASP.NET Core ���ø��� ����ؼ� ������Ʈ�� �����ϸ� View�� �ش��ϴ� Pages ������ �ڵ� �����ȴ�. Model�̳� Controller�� �츮�� �ʿ信 ���� �����ϸ� �ȴ�.

### Model
���� �츮�� ����� products.json ������ �����͸� ASP.NET Core ������Ʈ���� �ҷ��ͼ� ����ϱ� ���� �ش� �����͸� �ҷ��� Model�� �����غ���.

�� �׸��� product�� �Ǵ¼��̴� `Product.cs` �� ���̹��� �ؼ� Models ������ �����غ���.

�ʵ� ���Ǵ� �����ϰ� getter�� setter�� ������ public �ʵ�� �����ߴ�. �� �κ��� �츮 ������� �����ϸ� �� �� �ϴ�.

```cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestProject.WebSite.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Maker { get; set; }

        [JsonPropertyName("img")]
        public string Image { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int[]? Ratings { get; set; }

        public override string ToString() => JsonSerializer.Serialize<Product>(this);
    }
}
```

Java ���Ǹ� �������� �־ �׷��� ���� ������ �ڹٶ� ����ϴ�.

���� �����Ϳ� �ٸ� �ʵ���� ��Ī���ִ� ��ĵ� �׷���, �ν��Ͻ� ����� ���� �������̵��ϴ� ToString �޼��尡 ������ �ͼ��ϴ�.

`[JsonPropertyName("img")]` �ڵ�� �� ������Ʈ ���ο��� ����ϴ� Image��� �ʵ��� ���� �����Ϳ� ����� img �ʵ���� ��Ī�ϱ� ���ؼ� ����Ѵ�.

ToString �޼��忡�� ����ϴ� `JsonSerializer.Serialize<Product>(this);` �ڵ�� �Ķ���Ϳ� ���� ���� JSON string���� ��ȯ���ִ� ������ �Ѵ�.

���� �ƽ��� �� �Ѱ����� ����� import(using) �� �� �� �����׸��� ���� �ҷ��;� �ϴ����� �ǹ��̴�.

�̷��� ���� �����غ��Ҵ�. ���� ���� �����͸� �о �� Model ������ ����ϱ� ���ؼ��� json ������ �о�ͼ� �����ϴ� �ڵ尡 ���� �ʿ��ϴ�.

�̿� ���õ� �ڵ�� Services ������ �ڵ�� �ۼ��� ���̴�.

### Service
�����Ϳ��� Services ������ Add - Class �� ���� ����(`JsonFileProductService.cs`)�� �����ϰ� �ڵ带 �ۼ��ߴ�.

```cs
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
```

IWebHostEnvironment �� ���ø����̼��� ����ǰ��ִ� �� ȣ���� ȯ���� ������ �������ִ� �������̽��̴�.

�̸� �̿��� �ҷ��� ������ ������ �ִ� WebRootPath �� ����� �� �ְ�, `using(var jsonFileReader = File.OpenText(���))` �ڵ带 ���� ������ �о�� �� �ִ�.

���� �����͸� �о�ͼ� ��Ʈ������ �����, ���� ReadToEnd �޼��带 ���� string���� ��ȯ�� �Ŀ� JsonSerializer.Deserialize �޼��带 ���� json string �����͸� `IEnumerable<Product>` ���·� ��ȯ�� return �Ѵ�.

���⼭ generic type�� ���� Product �� �� Models�� �ۼ��� Product.cs�� �ִ� Ŭ������ �ҷ��� ���̴�.

���� AddRating �޼���� ����ڰ� ������ Item�� �ҷ��� �����Ϳ��� ã�Ƴ�(LINQ Ȱ��) ���� �� �ٽ� products.json ���Ͽ� ����� �Ѵ�.

�̷ν� ���� ���������� �ʿ��� ���񽺴� ��� �ۼ��ߴ�.

### View
���� ������Ʈ�� �����ϴ� Index �������� �����͸� �����ͼ� View�� ������Ʈ �� ���̴�.

�̸� ���ؼ��� ���� �������� �ڵ带 ���캸��. ���� ������ ������ ����ϴ�.

```cshtml
@page
@model IndexModel
@{
    ViewData["Title"] = "Indexx page";
}

<div class="text-center">
    <h1 class="display-4">Test Project2</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>
```

������ Ȯ���ڰ� cshtml �̰� �ڵ带 ���� html�� csharp �ڵ尡 ���� ���ǰ� �ִ�.

�̸� Razor View �����̶�� �θ��µ�, html�� csharp �ڵ带 ����ϱ� ���� `@`(at) ��ȣ�� ����Ѵ�.

�ֻ���� ���� �������� ��Ƽ����� ���ǰ� �ִ�. �� �� `@model` ��Ƽ�긦 ���� `IndexModel` �� �ҷ����� �ִµ�, �̴� Index.cshtml�� ���� ��ο� ��ġ�ϴ� Index.cshtml.cs ���Ͽ� �����ϴ� class �̴�.

Index.cshtml.cs ���ϰ��� ������ ���� �ڵ带 ������ ���̶� �θ��µ�, ���⿡ �������� ��µ� �����Ͱ� ����� �����Ѵ�.

�ε��� ������ ���� �ʱ� ������ ������ ����.

```cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestProject.WebSite.Pages
{
    public class IndexModel(ILogger<IndexModel> logger) : PageModel
    {

    }

    public void onGet()
    {
    
    }
}
```

onGet �޼��尡 ó������ �־����� ����� �ȳ��� ��,.��

��·�� �ڵ带 ���� IndexModel�� PageModel�� ��ӹ޴°� �� �� �ִ�.

�� constructor�� ���ڷμ� �ܼ��� ILogger �� �Ѱ��ִ� �� �����ε� �ΰ� ����� �ҷ��� Azure, Cloud ��� ���� �� �ִ�.

���� ���� ���񽺵鵵 �ҷ��ͼ� ���ڷ� �־��ָ� ��밡���ϴ�.

onGet �޼��忡�� �������� ���� �� ����� �ڵ带 �־��ָ� �ȴ�.

Services �� ������ ��ɵ��� �ҷ��� ����� �ڵ�� ������ ����.

```cs
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
```

���� Razor View ���Ͽ��� PageModel �� ���ǵ� Products �ʵ带 ����� �� �ִ�.

�̸� ���� �ε��� �������� ����� �ڵ带 �ۼ��غ���.

```cshtml
@page
@using TestProject.WebSite.Components
@model IndexModel
@{
    ViewData["Title"] = "Indexx page";
}

<div class="text-center">
    <h1 class="display-4">Test Project2</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>


<component type="typeof(ProductList)" render-mode="ServerPrerendered" />
```

## Blazor

���ο� application ��.

- Blazor �� ��ü apllication �μ� ����ص� �ǰ�, ������ ���ο����� ����ص� �ȴ�. ����� �����
- ���� ������ ������Ʈ ��� ����
- Visual Studio ���� Razor Component ���ø� ����

4���� ���ǿ����� Razor Component�� ����ϴµ� �ڵ尡 ������ ���� �ʾҴ�.

�Ʒ��� �ڵ带 ���캸��.

```razor
@page
@using TestProject.WebSite.Components
@model IndexModel
@{
    ViewData["Title"] = "Indexx page";
}

<div class="text-center">
    <h1 class="display-4">Test Project2</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>

@(await Html.RenderComponentAsync<ProductList>(RenderMode.ServerPrerendered))

<script src="_framework/blazor.server.js"></script>
```

������Ʈ�� ����ϱ� ���� `@using` ��Ƽ�긦 ����ߴ�. ������ �� ���� ������Ʈ ������ �ڵ��̴�.

����ϴ� �Լ��� Ŭ������ �����ϰ� �� �ϴ°��� ���Ⱑ �����.

4���� ���ǿ��� ����� �ڵ��̴� ����, ����� �� �� �������� �ʾ����� MS�� ��ť��Ʈ�� ã�ƺô�.

��ť��Ʈ�� �ڵ�� ���� �� �����ϱ� �ߴ�.

```cshtml
@page
@using TestProject.WebSite.Components
@model IndexModel
@{
    ViewData["Title"] = "Indexx page";
}

<div class="text-center">
    <h1 class="display-4">Test Project2</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>

<component type="typeof(ProductList)" render-mode="ServerPrerendered" />

<script src="_framework/blazor.server.js"></script>
```

�ϴ� ������Ʈ�� �������ϴ� �ڵ尡 �ξ� �������� ��������.

~~�׸��� �Ʒ��� ��ũ��Ʈ�� �ҷ����� �ڵ带 �����غ��Ҵµ� �ϴ� ���ۿ��� ������ ��� ���ξ���. ������ ����� �ٽ� ����.~~(����� �ȿ���....)

���ǿ��� ����ϴ� ���� ���ø��� 4���� ���ø��̴�. ���� ���ø��� ���� ������Ʈ �ƴµ�, ���ø����� ����ϴ� bootstrap�� ������ 5 �������� �Ѿ�鼭 �����ͼӼ��� ���ֿ̹� prefix�� �߰��ƴ�.

������ `data-toggle`, `data-target`, `data-dismiss` ���� ���ֿ̹��� `data-bs-toggle`, `data-bs-target`, `data-bs-dismiss` �̷������� bootstrap���� ����ϴ� ������ �Ӽ��̶�� �� �������� ���� bs(bootstrap)�̶�� prefix�� �߰��� �� �ϴ�.

`@for` ��Ƽ�� ��� �� ���ǻ���

```cshtml
@for(int star = 1; star < 6; star++)
{
    if (star <= selectedRating)
    {
        <span class="fa-star fa checked" @onmouseover="(e=>SetSelectedRating(star))" @onclick="(e=>SubmitRating(star))" />
    } else
    {
        <span class="fa-star fa" @onmouseover="(e=>SetSelectedRating(star))" @onclick="(e=>SubmitRating(star))" />
    }
}
```

�� �ڵ�� ���� �ý����� ���� �ڵ���̾���. ���콺�� �ø��� ���� ������ ������ ���� �޼��带 ���� ����ǰ�, Ŭ���ϸ� �ش� ������ ������Ʈ�ϴ� �����̴�.

����� ����ϸ� ����� �������� �ʾҴ�. �� �׷�����? ������� ���� ���� ���캸�� ������ 1������ 5������ ���õǱ⸦ �ٷ�����, **� ������ �����ϵ� �̺�Ʈ �ڵ鷯 �޼��忡 ���� star ���� ���� 6�̾���.**

�̴� `@for` ��Ƽ�긦 ���� �������� �ڵ忡�� star ������ ��� for loop�� ���� ������ star�� �����ϰ� �ֱ� �����̴�.

Razor Page�� ������ �Ǵ� ������ ������ ��Ȯ�� �˾ƺ��� ������, ��� ������� ���� ���캻 ����� �׷���.

�׳� �ڵ带 ¥�ٺ��� ����� ���������� `@for` ��Ƽ�긦 ����� �������Ǵ� �ϳ� �ϳ��� span �±װ� ���� �ش� ������ star ���� ����Ͽ� �������� �ɰŶ�� ����������, ���� ���忡���� ��� ���� star ������ �����ϸ� for loop �� �Ϸ�� ���� ������ �������� �Ϸ��Ѱ��̴�.

�̸� �ذ��ϱ� ���ؼ��� �ݺ��Ǵ� �ڵ�� ���ο��� ���������� ���� �����Ͽ� �ű⿡ �ش� ������ star ���� �����س��� ����ϴ� ���̴�.

�Ʒ��� �ڵ带 ���캸��.

```cshtml
@for(int star = 1; star < 6; star++)
{
    var currentStar = star;
    if (star <= selectedRating)
    {
        <span class="fa-star fa checked" @onmouseover="(e=>SetSelectedRating(currentStar))" @onclick="(e=>SubmitRating(currentStar))" />
    } else
    {
        <span class="fa-star fa" @onmouseover="(e=>SetSelectedRating(currentStar))" @onclick="(e=>SubmitRating(currentStar))" />
    }
}
```

�޶��� ���� �� �ϳ� star �� ���� currentStar ��� ������ ������ ���̴�. �� ������ ���������̰�, for loop�� ����ʿ� ���� ���� ���� �ʱ�ȭ �Ǵ� �����̱� ������ ���ϴ� ��� ������ �̲�� �� �־���.

## Publish project with Azure
�ϼ��� ������Ʈ�� Azure�� ���� �����غ���.

Visual Studio ������ ���� �����ϰ� ������ �����ϴ�.

Solution Explorer ���� ���� �� ������Ʈ `TestProject.WebSite` �� ��Ŭ���ϸ� publish ��� �޴��� ���� ���̴�.

�̸� ����ϸ� Azure, IIS, Folder �� ���� ������� ������ �����ϴ�.

MS �������� �α����ϰ� Subscription�� �����ϸ� �����ϰ� framework ������ publish�� �����ϴ�.(�ٵ� �� �����ϱ� �Ȱ� �ű԰��Ե� �������� �н�)

������ �������� `dotnet run` ��ɾ ���� �����ϸ� �ɵ��ϴ�. ���� ��δ� `Program.cs` �� �����ϴ� ������Ʈ ��Ʈ ��ο��� �����ϸ� �ȴ�. nginx�� ����ؼ� https ������ �����ϰ� �ϸ� �ɵ��ϴ�. ���� ASP.NET���� https�� ����ϴ� ����� �ִµ� �ʿ��ϸ� �˾ƺ���.(https://learn.microsoft.com/ko-kr/aspnet/core/security/enforcing-ssl?view=aspnetcore-8.0&tabs=visual-studio%2Clinux-ubuntu)

