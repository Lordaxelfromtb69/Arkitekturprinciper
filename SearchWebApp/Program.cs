// Program.cs (opdateret til at inkludere HttpClient)
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SearchWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Tilf�j HttpClient til at kalde SearchAPI
builder.Services.AddScoped<SearchService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new SearchService(httpClient); // S�rg for at returnere en v�rdi uanset hvad
});


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();