using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Gateway.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 1) Registrér HttpClient’er for hver back-end (ret portene til dine faktisk kørende services):
builder.Services.AddHttpClient("sent", client =>
    client.BaseAddress = new Uri("http://localhost:5099/")   // Snippets.Api HTTP
);
builder.Services.AddHttpClient("deleted", client =>
    client.BaseAddress = new Uri("http://localhost:5021/")   // Deleted.Api HTTP
);

// 2) Swagger/OpenAPI til udvikling
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3) Swagger UI kun i Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4) Health-check (valgfrit, men anbefales)
app.MapGet("/health", () => Results.Ok("Healthy"));

// 5) Aggregator-endpoint: /all-snippets/search/{term}
app.MapGet("/all-snippets/search/{term}", async (string term, IHttpClientFactory http, ILogger<Program> log) =>
{
    var results = new List<SnippetResult>();
    foreach (var name in new[] { "sent", "deleted" })
    {
        var client = http.CreateClient(name);
        try
        {
            // Husk absolut sti (leder efter /snippets/search/{term})
            var partial = await client.GetFromJsonAsync<List<SnippetResult>>(
                $"/snippets/search/{Uri.EscapeDataString(term)}",
                default
            );
            if (partial != null)
                results.AddRange(partial);
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            log.LogWarning(ex, "Call to {Service} failed", name);
        }
    }
    return Results.Ok(results);
});

app.Run();
