# TestProject
ASP.NET로 웹 프로젝트 만들어보기.

샘플 프로젝트 링크 : https://github.com/dotnet-presentations/ContosoCrafts

## MVC

관심사를 분리하는 측면의 용어이다. Model 을 사용해 DB와 소통을 한다. View 는 따로 아는 정보 없이 그저 정보나 데이터를 출력하는 역할만 한다. Controller 는 DB와 View 사이의 일종의 조율자의 역할을 한다.

웹 프로젝트 루트폴더에 Models, Pages, Controllers 폴더를 생성하고 각자 역할에 맞는 코드를 작성한다. 그 외에 애플리케이션 내에서 사용할 커스텀 기능들을 Services 폴더에 작성했다.

먼저 샘플 프로젝트는 products.json 파일 데이터를 DB 대용으로 사용하고 있으며 파일 내의 Ratings 라는 항목에 사용자와의 상호작용을 통해 평점을 추가하는 것이 목표이다.

인덱스 페이지에서 해당 파일의 내용들을 출력해주고, 각 항목의 버튼을 클릭 시 상세 조회와 별점 기능을 제공하는 모달을 띄우도록 만든다.

여기서 Models 폴더에는 products.json 이 가지고 있는 데이터의 구조를 코드로 작성해줘야 하고,

Controllers 폴더에서는 원래 서버와 api 통신을 통해 데이터를 불러오고 수정해야 하지만 로컬 환경이기 때문에 바로 Services에 작성한 코드를 사용하고있다.

Pages(Views) 폴더에서는 url 경로에 따른 뷰를 페이지 단위로 작성해놓았다.

ASP.NET Core 템플릿을 사용해서 프로젝트를 생성하면 View에 해당하는 Pages 폴더는 자동 생성된다. Model이나 Controller는 우리의 필요에 따라 생성하면 된다.

### Model
먼저 우리가 사용할 products.json 파일의 데이터를 ASP.NET Core 프로젝트에서 불러와서 사용하기 위해 해당 데이터를 불러올 Model을 정의해보자.

각 항목은 product가 되는셈이니 `Product.cs` 로 네이밍을 해서 Models 폴더에 생성해보자.

필드 정의는 간단하게 getter와 setter를 가지는 public 필드로 정의했다. 이 부분은 우리 마음대로 설정하면 될 듯 하다.

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

Java 강의를 들은적이 있어서 그런지 뭔가 굉장히 자바랑 비슷하다.

실제 데이터와 다른 필드명을 매칭해주는 방식도 그렇고, 인스턴스 출력을 위해 오버라이드하는 ToString 메서드가 굉장히 익숙하다.

`[JsonPropertyName("img")]` 코드는 웹 프로젝트 내부에서 사용하는 Image라는 필드명과 실제 데이터에 저장된 img 필드명을 매칭하기 위해서 사용한다.

ToString 메서드에서 사용하는 `JsonSerializer.Serialize<Product>(this);` 코드는 파라미터에 들어온 값을 JSON string으로 변환해주는 역할을 한다.

뭔가 아쉬운 점 한가지는 기능을 import(using) 할 때 왜 하위항목을 따로 불러와야 하는지가 의문이다.

이렇게 모델을 정의해보았다. 물론 실제 데이터를 읽어서 이 Model 구조로 사용하기 위해서는 json 파일을 읽어와서 저장하는 코드가 따로 필요하다.

이와 관련된 코드는 Services 폴더에 코드로 작성할 것이다.

### Service
에디터에서 Services 폴더에 Add - Class 를 통해 파일(`JsonFileProductService.cs`)을 생성하고 코드를 작성했다.

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

IWebHostEnvironment 는 애플리케이션이 실행되고있는 웹 호스팅 환경의 정보를 제공해주는 인터페이스이다.

이를 이용해 불러올 데이터 파일이 있는 WebRootPath 를 사용할 수 있고, `using(var jsonFileReader = File.OpenText(경로))` 코드를 통해 파일을 읽어올 수 있다.

문자 데이터를 읽어와서 스트림으로 만들고, 이후 ReadToEnd 메서드를 통해 string으로 변환한 후에 JsonSerializer.Deserialize 메서드를 통해 json string 데이터를 `IEnumerable<Product>` 형태로 변환해 return 한다.

여기서 generic type에 사용된 Product 는 위 Models에 작성한 Product.cs에 있는 클래스를 불러온 것이다.

이후 AddRating 메서드는 사용자가 선택한 Item을 불러온 데이터에서 찾아내(LINQ 활용) 수정 후 다시 products.json 파일에 덮어쓰기 한다.

이로써 서버 내부적으로 필요한 서비스는 모두 작성했다.

### View
현재 프로젝트에 존재하는 Index 페이지에 데이터를 가져와서 View를 업데이트 할 것이다.

이를 위해서는 먼저 페이지의 코드를 살펴보자. 대충 구성은 다음과 비슷하다.

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

파일의 확장자가 cshtml 이고 코드를 보면 html과 csharp 코드가 같이 사용되고 있다.

이를 Razor View 파일이라고 부르는데, html에 csharp 코드를 사용하기 위해 `@`(at) 기호를 사용한다.

최상단을 보면 여러가지 디렉티브들이 사용되고 있다. 그 중 `@model` 디렉티브를 보면 `IndexModel` 을 불러오고 있는데, 이는 Index.cshtml과 같은 경로에 위치하는 Index.cshtml.cs 파일에 존재하는 class 이다.

Index.cshtml.cs 파일같이 페이지 뒤의 코드를 페이지 모델이라 부르는데, 여기에 페이지에 출력될 데이터가 어떤건지 정의한다.

인덱스 페이지 모델의 초기 구성은 다음과 같다.

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

onGet 메서드가 처음부터 있었는지 기억이 안난다 ㅡ,.ㅡ

어쨌든 코드를 보면 IndexModel이 PageModel을 상속받는걸 볼 수 있다.

또 constructor의 인자로서 단순히 ILogger 를 넘겨주는 것 만으로도 로거 기능을 불러와 Azure, Cloud 등에서 사용될 수 있다.

따로 만든 서비스들도 불러와서 인자로 넣어주면 사용가능하다.

onGet 메서드에는 페이지가 열릴 때 실행될 코드를 넣어주면 된다.

Services 에 구현한 기능들을 불러와 사용한 코드는 다음과 같다.

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

이제 Razor View 파일에서 PageModel 에 정의된 Products 필드를 사용할 수 있다.

이를 통해 인덱스 페이지에 출력할 코드를 작성해보자.

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

새로운 application 모델.

- Blazor 를 전체 apllication 로서 사용해도 되고, 페이지 내부에서만 사용해도 된다. 사용자 맘대로
- 재사용 가능한 컴포넌트 기능 제공
- Visual Studio 에서 Razor Component 템플릿 제공

4년전 강의에서는 Razor Component를 사용하는데 코드가 마음에 들지 않았다.

아래의 코드를 살펴보자.

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

컴포넌트를 사용하기 위해 `@using` 디렉티브를 사용했다. 문제는 맨 밑의 컴포넌트 렌더링 코드이다.

사용하는 함수와 클래스도 난해하고 뭘 하는건지 보기가 힘들다.

4년전 강의에서 사용한 코드이다 보니, 현재는 좀 더 나아지지 않았을까 MS의 도큐먼트를 찾아봤다.

도큐먼트의 코드는 조금 더 간단하긴 했다.

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

일단 컴포넌트를 렌더링하는 코드가 훨씬 가독성이 좋아졌다.

~~그리고 아래의 스크립트를 불러오는 코드를 제거해보았는데 일단 동작에는 문제가 없어서 빼두었다. 문제가 생기면 다시 넣자.~~(모달이 안열려....)

강의에서 사용하던 개발 템플릿은 4년전 템플릿이다. 현재 템플릿은 많이 업데이트 됐는데, 템플릿에서 사용하는 bootstrap의 버전이 5 버전으로 넘어가면서 데이터속성의 네이밍에 prefix가 추가됐다.

기존의 `data-toggle`, `data-target`, `data-dismiss` 등의 네이밍에서 `data-bs-toggle`, `data-bs-target`, `data-bs-dismiss` 이런식으로 bootstrap에서 사용하는 데이터 속성이라는 걸 구분짓기 위해 bs(bootstrap)이라는 prefix를 추가한 듯 하다.

`@for` 디렉티브 사용 시 주의사항

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

위 코드는 별점 시스템을 위한 코드블럭이었다. 마우스를 올리면 내가 선택한 별점이 세터 메서드를 통해 변경되고, 클릭하면 해당 별점을 업데이트하는 형태이다.

결과만 얘기하면 제대로 동작하지 않았다. 왜 그랬을까? 디버깅을 통해 값을 살펴보니 별점을 1점부터 5점까지 세팅되기를 바랬지만, **어떤 별점을 선택하든 이벤트 핸들러 메서드에 들어온 star 값은 전부 6이었다.**

이는 `@for` 디렉티브를 통한 렌더링된 코드에서 star 변수가 모두 for loop가 끝난 시점의 star를 참조하고 있기 때문이다.

Razor Page가 렌더링 되는 과정이 뭔지는 정확히 알아봐야 겠지만, 적어도 디버깅을 통해 살펴본 결과는 그렇다.

그냥 코드를 짜다보니 사람의 시점에서는 `@for` 디렉티브를 사용해 렌더링되는 하나 하나의 span 태그가 각각 해당 시점의 star 값을 사용하여 렌더링이 될거라고 생각했지만, 서버 입장에서는 모두 같은 star 변수를 참조하며 for loop 가 완료된 후의 값으로 렌더링을 완료한것이다.

이를 해결하기 위해서는 반복되는 코드블럭 내부에서 지역변수를 따로 생성하여 거기에 해당 시점의 star 값을 저장해놓고 사용하는 것이다.

아래의 코드를 살펴보자.

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

달라진 것은 딱 하나 star 의 값을 currentStar 라는 변수에 저장한 것이다. 이 변수는 지역변수이고, for loop가 진행됨에 따라 각각 새로 초기화 되는 변수이기 때문에 원하는 대로 동작을 이끌어낼 수 있었다.

## Publish project with Azure
완성한 프로젝트를 Azure를 통해 배포해보자.

Visual Studio 에서는 아주 간단하게 배포가 가능하다.

Solution Explorer 에서 나의 웹 프로젝트 `TestProject.WebSite` 를 우클릭하면 publish 라는 메뉴가 보일 것이다.

이를 사용하면 Azure, IIS, Folder 등 여러 방식으로 배포가 가능하다.

MS 계정으로 로그인하고 Subscription을 선택하면 간단하게 framework 설정과 publish가 가능하다.(근데 난 결제하기 싫고 신규가입도 귀찮으니 패스)

리눅스 서버에는 `dotnet run` 명령어를 통해 실행하면 될듯하다. 실행 경로는 `Program.cs` 가 존재하는 웹사이트 루트 경로에서 실행하면 된다. nginx를 사용해서 https 설정은 동일하게 하면 될듯하다. 따로 ASP.NET에서 https를 사용하는 방법도 있는데 필요하면 알아보자.(https://learn.microsoft.com/ko-kr/aspnet/core/security/enforcing-ssl?view=aspnetcore-8.0&tabs=visual-studio%2Clinux-ubuntu)

