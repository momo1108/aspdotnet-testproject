using System.Text.Json;
using TestProject.WebSite.Models;
using TestProject.WebSite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<JsonFileProductService>(); // Ŀ���� ���񽺸� �߰��Ѵ�.
builder.Services.AddControllers(); // Ŀ���� Controller�� �߰����ش�.
builder.Services.AddServerSideBlazor(); // ������ ��� �� �߰����ش�.

// �⺻���� �帧 : 1. Ŀ���� ��� �߰� 2. Ŀ���� ��� ���

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

app.MapRazorPages(); // Pages�� Index.cshtml, Privacy.cshtml ���� ��������.
app.MapControllers(); // Controllers �� controller ���� ��������.
app.MapBlazorHub();

// ������ api �����. ������ ����� �ƴϴ�.
// Program.cs ���ο� API�� ���� �ڵ���� �Ѳ����� ���°� ���� �ʴ�.
//app.MapGet("/products", (context) =>
//{
//    var products = app.Services.GetService<JsonFileProductService>().GetProducts();
//    var json = JsonSerializer.Serialize<IEnumerable<Product>>(products);
//    return context.Response.WriteAsync(json);
//});

app.Run();
