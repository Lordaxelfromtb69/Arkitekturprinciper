using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SearchWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// HttpClient til API-opkald
builder.Services.AddHttpClient<SearchService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000"); // Din SearchAPI-base
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
