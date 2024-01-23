# TestProject
ASP.NET로 웹 프로젝트 만들어보기.

샘플 프로젝트 링크 : https://github.com/dotnet-presentations/ContosoCrafts

## MVC

관심사를 분리하는 측면의 용어이다. Model 을 사용해 DB와 소통을 한다. View 는 따로 아는 정보 없이 그저 정보나 데이터를 출력하는 역할만 한다. Controller 는 DB와 View 사이의 일종의 조율자의 역할을 한다.

## Blazor

새로운 application 모델.

- Blazor 를 전체 apllication 로서 사용해도 되고, 페이지 내부에서만 사용해도 된다. 사용자 맘대로
- 재사용 가능한 컴포넌트 기능 제공
- Visual Studio 에서 Razor Component 템플릿 제공

4년전 강의에서는 Razor Component를 사용하는데 코드가 마음에 들지 않았다.

아래의 코드를 살펴보자.

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
```

일단 컴포넌트를 렌더링하는 코드가 훨씬 가독성이 좋아졌다.

그리고 아래의 스크립트를 불러오는 코드를 제거해보았는데 일단 동작에는 문제가 없어서 빼두었다. 문제가 생기면 다시 넣자.

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