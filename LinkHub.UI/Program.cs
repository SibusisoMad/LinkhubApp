
using LinkHub.UI.Models.Interfaces;
using LinkHub.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient for API integration
builder.Services.AddHttpClient("ApiClient", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config.GetSection("ApiSettings:BaseUrl").Value;
    if (!string.IsNullOrEmpty(baseUrl))
        client.BaseAddress = new Uri(baseUrl);
});

// Register ClientService for DI
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IContactService, ContactService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
