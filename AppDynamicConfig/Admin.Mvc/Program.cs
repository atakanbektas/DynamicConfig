using Admin.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<ConfigApiClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5080";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Configs}/{action=Index}/{id?}");

app.Run();
