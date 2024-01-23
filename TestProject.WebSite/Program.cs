using System.Text.Json;
using TestProject.WebSite.Models;
using TestProject.WebSite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<JsonFileProductService>(); // 커스텀 서비스를 추가한다.
builder.Services.AddControllers(); // 커스텀 Controller를 추가해준다.
builder.Services.AddServerSideBlazor(); // 블레이저 사용 시 추가해준다.

// 기본적인 흐름 : 1. 커스텀 기능 추가 2. 커스텀 기능 사용

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages(); // Pages의 Index.cshtml, Privacy.cshtml 등을 매핑해줌.
app.MapControllers(); // Controllers 의 controller 들을 매핑해줌.
app.MapBlazorHub();

// 간단한 api 만들기. 간단한 방식이 아니다.
// Program.cs 내부에 API나 여러 코드들을 한꺼번에 놓는건 좋지 않다.
//app.MapGet("/products", (context) =>
//{
//    var products = app.Services.GetService<JsonFileProductService>().GetProducts();
//    var json = JsonSerializer.Serialize<IEnumerable<Product>>(products);
//    return context.Response.WriteAsync(json);
//});

app.Run();
