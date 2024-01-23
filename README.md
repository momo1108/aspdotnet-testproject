# TestProject
ASP.NET�� �� ������Ʈ ������.

���� ������Ʈ ��ũ : https://github.com/dotnet-presentations/ContosoCrafts

## MVC

���ɻ縦 �и��ϴ� ������ ����̴�. Model �� ����� DB�� ������ �Ѵ�. View �� ���� �ƴ� ���� ���� ���� ������ �����͸� ����ϴ� ���Ҹ� �Ѵ�. Controller �� DB�� View ������ ������ �������� ������ �Ѵ�.

## Blazor

���ο� application ��.

- Blazor �� ��ü apllication �μ� ����ص� �ǰ�, ������ ���ο����� ����ص� �ȴ�. ����� �����
- ���� ������ ������Ʈ ��� ����
- Visual Studio ���� Razor Component ���ø� ����

4���� ���ǿ����� Razor Component�� ����ϴµ� �ڵ尡 ������ ���� �ʾҴ�.

�Ʒ��� �ڵ带 ���캸��.

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
```

�ϴ� ������Ʈ�� �������ϴ� �ڵ尡 �ξ� �������� ��������.

�׸��� �Ʒ��� ��ũ��Ʈ�� �ҷ����� �ڵ带 �����غ��Ҵµ� �ϴ� ���ۿ��� ������ ��� ���ξ���. ������ ����� �ٽ� ����.

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